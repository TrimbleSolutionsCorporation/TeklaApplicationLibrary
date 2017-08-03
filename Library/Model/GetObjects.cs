using System.Collections.Generic;
using TSM = Tekla.Structures.Model;
using TS = Tekla.Structures;

namespace Tekla.Structures.Model
{
    public static class GetObjects
    {
        /// <summary>
        /// Gets a list with a single model object selected by GUID
        /// </summary>
        /// <param name="Guid">Tekla object GUID string</param>
        /// <returns>List of tekla model objects or empty list if invalid GUID</returns>
        public static List<TSM.ModelObject> Get(string Guid)
        { return Get(new TSM.Model().GetIdentifierByGUID(Guid)); }

        /// <summary>
        /// Gets a list with model objects selected by GUID
        /// </summary>
        /// <param name="Guids">Tekla object GUID strings list</param>
        /// <returns>List of tekla model objects or empty list if invalid GUIDs</returns>
        public static List<TSM.ModelObject> Get(List<string> Guids)
        {
            List<TS.Identifier> lst = new List<TS.Identifier>();

            Guids.ForEach(x => lst.Add(new TSM.Model().GetIdentifierByGUID(x)));

            return Get(lst);
        }

        /// <summary>
        /// Gets a list with model objects selected by int ID
        /// </summary>
        /// <param name="Id">Tekla object int ID</param>
        /// <returns>List of tekla model objects or empty list if invalid ID</returns>
        public static List<TSM.ModelObject> Get(int Id)
        { return Get(new TS.Identifier(Id)); }

        /// <summary>
        /// Gets a list with model objects selected by int IDs
        /// </summary>
        /// <param name="Ids">Tekla object int IDs list</param>
        /// <returns>List of tekla model objects or empty list if invalid IDs</returns>
        public static List<TSM.ModelObject> Get(List<int> Ids)
        {
            TSM.UI.ModelObjectSelector sel = new TSM.UI.ModelObjectSelector();

            List<TS.Identifier> list = new List<TS.Identifier>();

            Ids.ForEach(x => list.Add(new TS.Identifier(x)));

            return Get(list);
        }

        /// <summary>
        /// Gets a list with model objects selected by tekla identifier
        /// </summary>
        /// <param name="Identifier">Tekla object Identifier</param>
        /// <returns>List of tekla model objects or empty list if invalid Identifier</returns>
        public static List<TSM.ModelObject> Get(TS.Identifier Identifier)
        { return Get(new List<TS.Identifier>() { Identifier }); }

        /// <summary>
        /// Gets a list with model objects selected by tekla identifiers
        /// </summary>
        /// <param name="Identifiers">Tekla object identifier list</param>
        /// <returns>List of tekla model objects or empty list if invalid identifiers</returns>
        public static List<TSM.ModelObject> Get(List<TS.Identifier> Identifiers)
        {
            List<TSM.ModelObject> lst = new List<TSM.ModelObject>();
            Identifiers.ForEach(x => lst.Add(new TSM.Model().SelectModelObject(x)));

            return lst;
        }
    }
}
