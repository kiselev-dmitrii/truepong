﻿using System;

namespace TruePong.Utils {
    public static class ActionUtils {
        public static void TryCall(this Action self) {
            if (self != null) self();
        }

        public static void TryCall<T1>(this Action<T1> self, T1 arg1) {
            if (self != null) self(arg1);
        }

        public static void TryCall<T1, T2>(this Action<T1, T2> self, T1 arg1, T2 arg2) {
            if (self != null) self(arg1, arg2);
        }

        public static void TryCall<T1, T2, T3>(this Action<T1, T2, T3> self, T1 arg1, T2 arg2, T3 arg3) {
            if (self != null) self(arg1, arg2, arg3);
        }
    }
}
