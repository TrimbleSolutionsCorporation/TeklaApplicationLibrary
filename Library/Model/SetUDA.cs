using System.Collections.Generic;
using TSM = Tekla.Structures.Model;
using TS = Tekla.Structures;

namespace Tekla.Structures.Model
{
    public static partial class UDA
    {
        public static void SetAllUDA(string Guid, Dictionary<string, object> UdaList)
            => SetAllUDA(GetObject.Get(Guid), UdaList);

        public static void SetAllUDA(int Id, Dictionary<string, object> UdaList)
           => SetAllUDA(GetObject.Get(Id), UdaList);

        public static void SetAllUDA(TS.Identifier Identifier, Dictionary<string, object> UdaList)
           => SetAllUDA(GetObject.Get(Identifier), UdaList);

        public static void SetAllUDA(TSM.ModelObject modelObject, Dictionary<string, object> UdaList)
        {
            foreach(var item in UdaList)
            {
                if(item.Value != null)
                {
                    if(item.Value is string)
                        modelObject.SetUserProperty(item.Key, (string)item.Value);
                    else if(item.Value is int)
                        modelObject.SetUserProperty(item.Key, (int)item.Value);
                    else if(item.Value is double)
                        modelObject.SetUserProperty(item.Key, (double)item.Value);
                }
            }

            modelObject.Modify();
        }
    }
}
