using System.Collections.Generic;
using TSM = Tekla.Structures.Model;
using TS = Tekla.Structures;

namespace Tekla.Structures.Model
{
    public static partial class UDA
    {
        public static object GetUDA(string Guid, string Uda)
        { return GetUDA(GetObject.Get(Guid), Uda); }

        public static object GetUDA(int Id, string Uda)
        { return GetUDA(GetObject.Get(Id), Uda); }

        public static object GetUDA(TS.Identifier Identifier, string Uda)
        { return GetUDA(GetObject.Get(Identifier), Uda); }

        public static object GetUDA(TSM.ModelObject ModelObj, string Uda)
        {
            string refString = "";

            ModelObj.GetUserProperty(Uda, ref refString);

            if (refString.Length < 1)
            {
                double refDouble = double.NaN;

                ModelObj.GetUserProperty(Uda, ref refDouble);

                if (double.IsNaN(refDouble))
                {
                    int refInt = int.MinValue;

                    ModelObj.GetUserProperty(Uda, ref refInt);

                    if (refInt != int.MinValue)
                        return refInt;

                    return null;
                }
                else
                {
                    return refDouble;
                }
            }
            else
            {
                return refString;
            }
        }

        public static Dictionary<string, object> GetUDAs(string Guid, List<string> Udas)
        { return GetUDAs(GetObject.Get(Guid), Udas); }

        public static Dictionary<string, object> GetUDAs(int Id, List<string> Udas)
        { return GetUDAs(GetObject.Get(Id), Udas); }

        public static Dictionary<string, object> GetUDAs(TS.Identifier Identifier, List<string> Udas)
        { return GetUDAs(GetObject.Get(Identifier), Udas); }

        public static Dictionary<string, object> GetUDAs(TSM.ModelObject ModelObj, List<string> Udas)
        {
            Dictionary<string, object> Items = new Dictionary<string, object>();

            foreach (var item in Udas)
                Items.Add(item, GetUDA(ModelObj, item));

            return Items;
        }
    }
}
