package soc.server.genericServer;

import soc.disableDebug.D;
import soc.message.SOCMessage;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.Serializable;
import java.net.Socket;
import java.util.Date;
import java.util.Vector;

final class NetConnection extends Connection implements Runnable, Serializable, Cloneable {

    private static final int TIMEOUT_VALUE = 60 * 60 * 1000;

    private DataInputStream in;
    private DataOutputStream out;
    private Socket socket;

    private String hostname;
    private boolean connected = false;
    private boolean inputConnected = false;

    private final Vector<String> outQueue = new Vector<>();

    public NetConnection(Socket socket, Server server) {
        this.hostname = socket.getInetAddress().getHostName();
        this.socket = socket;
        this.ourServer = server;
    }

    public String getName() {
        if (hostname != null && socket != null) {
            return "connection-" + hostname + "-" + socket.getPort();
        } else {
            return "connection-(null)-" + hashCode();
        }
    }

    public String host() {
        return hostname;
    }

    public boolean connect() {
        if (getData() != null) {
            D.ebugPrintlnINFO("conn.connect() requires null getData()");
            return false;
        }
        try {
            socket.setSoTimeout(TIMEOUT_VALUE);
            in = new DataInputStream(socket.getInputStream());
            out = new DataOutputStream(socket.getOutputStream());
            connected = true;
            inputConnected = true;
            connectTime = new Date();
            new Putter().start();
        } catch (IOException e) {
            D.ebugPrintlnINFO("IOException in NetConnection.connect - " + e);
            error = e;
            disconnect();
            return false;
        }
        return true;
    }

    public boolean isInputAvailable() {
        try {
            return inputConnected && in.available() > 0;
        } catch (IOException e) {
            return false;
        }
    }

    public void run() {
        Thread.currentThread().setName(getName());
        ourServer.addConnection(this);
        try {
            if (inputConnected) {
                SOCMessage firstMsg = SOCMessage.toMsg(in.readUTF());
                if (!ourServer.processFirstCommand(firstMsg, this)) {
                    if (firstMsg != null) ourServer.inQueue.push(firstMsg, this);
                }
            }
            while (inputConnected) {
                SOCMessage msg = SOCMessage.toMsg(in.readUTF());
                if (msg != null) ourServer.inQueue.push(msg, this);
            }
        } catch (Exception e) {
            D.ebugPrintlnINFO("Exception in NetConnection.run - " + e);
            error = e;
            ourServer.removeConnection(this, false);
        }
    }

    public void put(String str) {
        synchronized (outQueue) {
            outQueue.addElement(str);
            outQueue.notify();
        }
    }

    private boolean putForReal(String str) {
        if (!putAux(str)) {
            if (connected) {
                ourServer.removeConnection(this, false);
            }
            return false;
        }
        return true;
    }

    private boolean putAux(String str) {
        if (error != null || !connected) return false;
        try {
            out.writeUTF(str);
        } catch (IOException e) {
            D.ebugPrintlnINFO("IOException in NetConnection.putAux - " + e);
            error = e;
            return false;
        } catch (Exception e) {
            D.ebugPrintlnINFO("Generic exception in NetConnection.putAux");
            return false;
        }
        return true;
    }

    public void disconnect() {
        if (!connected) return;
        connected = false;
        inputConnected = false;
        try {
            if (out != null) out.flush();
            if (socket != null) socket.close();
        } catch (IOException e) {
            D.ebugPrintlnINFO("IOException in NetConnection.disconnect - " + e);
            error = e;
        }
        socket = null;
        in = null;
        out = null;
    }

    public void disconnectSoft() {
        if (!inputConnected) return;
        inputConnected = false;
        try {
            if (out != null) out.flush();
        } catch (IOException ignored) {}
    }

    public boolean isConnected() {
        return connected && inputConnected;
    }

    @Override
    public String toString() {
        return "Connection[" + (data != null ? data : hashCode()) + "-" + getName() + "]";
    }

    class Putter extends Thread {
        public Putter() {
            String cn = host();
            setName((cn != null ? "putter-" + cn + "-" + socket.getPort() : "putter-(null)-" + hashCode()));
        }

        public void run() {
            while (connected) {
                String c = null;
                synchronized (outQueue) {
                    if (!outQueue.isEmpty()) {
                        c = outQueue.remove(0);
                    }
                }
                if (c != null) {
                    putForReal(c);
                }
                synchronized (outQueue) {
                    if (outQueue.isEmpty()) {
                        try {
                            outQueue.wait(1000);
                        } catch (InterruptedException ignored) {}
                    }
                }
            }
        }
    }
}
