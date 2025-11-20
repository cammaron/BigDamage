using HarmonyLib;
using System;
using System.Linq.Expressions;

namespace BigDamage.Patches
{
    /* Convenience methods for easy compatibility between HarmonyX versions (at time of writing, nightly build API has changed to require CodeMatch instead of CodeInstruction in matchers, and CodeMatch has its own static methods matching the names below) */
    public static class _Util
    {
        public static CodeMatch LoadsField(System.Reflection.FieldInfo field)
        {
            return new CodeMatch(CodeInstruction.LoadField(field.DeclaringType, field.Name));
        }

        public static CodeMatch Calls(Expression<Action> expression)
        {
            return new CodeMatch(CodeInstruction.Call(expression));
        }

    }
}
