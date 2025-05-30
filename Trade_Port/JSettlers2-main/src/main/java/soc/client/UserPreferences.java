
        package soc.client;
import java.util.HashMap;
import java.util.Map;


public class UserPreferences {

    private static final Map<String, Object> prefs = new HashMap<>();

    static {
        // Default preferences
        prefs.put("soundOn", true);
        prefs.put("botTradeRejectSec", 5);
        prefs.put("newGameRandomBoard", true);
        prefs.put("newGameUseRobber", true);
        prefs.put("newGameVictoryPoints", 10);
        prefs.put("newGameUseFriendlyRobber", false);
        prefs.put("newGameMaxPlayers", 4);
        prefs.put("newGameGameType", 0);
    }

    public static boolean getPref(final String prefKey, final boolean dflt) {
        Object val = prefs.get(prefKey);
        if (val instanceof Boolean) {
            return (Boolean) val;
        }
        return dflt;
    }

    public static int getPref(final String prefKey, final int dflt) {
        Object val = prefs.get(prefKey);
        if (val instanceof Integer) {
            return (Integer) val;
        }
        return dflt;
    }

    public static void putPref(final String prefKey, final boolean val) {
        prefs.put(prefKey, val);
    }

    public static void putPref(final String prefKey, final int val) {
        prefs.put(prefKey, val);
    }

    /**
     * Clear specific preferences based on a comma-separated key list.
     * @param prefKeyList Comma-separated keys to remove from prefs.
     */
    public static void clear(final String prefKeyList) {
        if (prefKeyList == null || prefKeyList.isEmpty()) return;
        for (String key : prefKeyList.split(",")) {
            prefs.remove(key.trim());
        }
        System.err.println("Cleared hardcoded preferences: " + prefKeyList);
    }
}

