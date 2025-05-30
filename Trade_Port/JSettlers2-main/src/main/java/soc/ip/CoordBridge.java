package soc.ip;


import java.util.*;

import soc.ip.Point;


public class CoordBridge {

    public static int[] addV = {0x01, 0x12, 0x21, 0x10, -0x01, -0x10};
    public static int[] addE = {0x01, 0x11, 0x10, -0x01, -0x11, -0x10};


    public static HashMap<Point<Integer, Integer>, Integer> backToAiCode = new HashMap<>();
    public static HashMap<Integer, Point<Integer, Integer>> aiCodeToBack = new HashMap<>();
    private static final Map<Integer, String> edgeToCoords = new HashMap<>();

    private static final int[][] HEXES = {
            {-2, 2}, {-1, 2}, {0, 2},
            {-2, 1}, {-1, 1}, {0, 1}, {1, 1},
            {-2, 0}, {-1, 0}, {0, 0}, {1, 0}, {2, 0},
            {-1, -1}, {0, -1}, {1, -1}, {2, -1},
            {0, -2}, {1, -2}, {2, -2}
    };

    private static final int[] baseCodes = {
            0x37, 0x59, 0x7B,
            0x35, 0x57, 0x79, 0x9B,
            0x33, 0x55, 0x77, 0x99, 0xBB,
            0x53, 0x75, 0x97, 0xB9,
            0x73, 0x95, 0xB7
    };

    static {
        for (int i = 0; i < HEXES.length; i++) {
            int x = HEXES[i][0];
            int y = HEXES[i][1];
            int base = baseCodes[i];

            Point<Integer, Integer> pos = new Point<>(x, y);
            backToAiCode.put(pos, base);
            aiCodeToBack.put(base, pos);

        }

    }

    public static String getVertex(int code) {
        for (Map.Entry<Integer, Point<Integer, Integer>> e : aiCodeToBack.entrySet()) {
            int base = e.getKey();
            Point<Integer, Integer> pos = e.getValue();
            for (int i = 0; i < addV.length; i++) {
                if (base + addV[i] == code) {
                    return pos.getA() + " " + pos.getB() + " " + i;
                }
            }
        }
        return "ERROR";
    }


    public static String getEdge(int code) {
        for (Map.Entry<Integer, Point<Integer, Integer>> e : aiCodeToBack.entrySet()) {
            int base = e.getKey();
            Point<Integer, Integer> pos = e.getValue();
            for (int i = 0; i < addE.length; i++) {
                if (base + addE[i] == code) {
                    return pos.getA() + " " + pos.getB() + " " + i;
                }
            }
        }
        return "ERROR";
    }



    public static int getVertex(int x, int y, int d) {
        return backToAiCode.get(new Point<>(x, y)) + addV[d];
    }

    public static int getEdge(int x, int y, int d) {
        return backToAiCode.get(new Point<>(x, y)) + addE[d];
    }


}
