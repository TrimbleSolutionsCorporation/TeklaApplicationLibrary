using System.Collections.Generic;
using TSM = Tekla.Structures.Model;
using TS = Tekla.Structures;
using System.Collections;

namespace Tekla.Structures.Model
{
    /// <summary>
    /// Static class containing Tekla Structures model object selectors
    /// </summary>
    public static class SelectObjects
    {
        /// <summary>
        /// Selects model object with the given GUID
        /// </summary>
        /// <param name="Guid">Tekla object GUID</param>
        public static void Select(string Guid) =>
            Select(new TSM.Model().GetIdentifierByGUID(Guid));

        /// <summary>
        /// Selects model objects with the given GUIDs
        /// </summary>
        /// <param name="Guids">Tekla object GUIDs list</param>
        public static void Select(List<string> Guids)
        {
            List<TS.Identifier> lst = new List<TS.Identifier>();

            Guids.ForEach(x => lst.Add(new TSM.Model().GetIdentifierByGUID(x)));

            Select(lst);
        }

        /// <summary>
        /// Selects model objects with the given ID
        /// </summary>
        /// <param name="Id">Tekla object ID</param>
        public static void Select(int Id) =>
            Select(new TS.Identifier(Id));

        /// <summary>
        /// Selects model objects with the given IDs
        /// </summary>
        /// <param name="Ids">Tekla object IDs list</param>
        public static void Select(List<int> Ids)
        {
            TSM.UI.ModelObjectSelector sel = new TSM.UI.ModelObjectSelector();

            List<TS.Identifier> list = new List<TS.Identifier>();

            Ids.ForEach(x => list.Add(new TS.Identifier(x)));

            Select(list);
        }

        /// <summary>
        /// Selects model objects with the given Identifier
        /// </summary>
        /// <param name="Identifier">Tekla object Identifier</param>
        public static void Select(TS.Identifier Identifier) =>
            Select(new List<TS.Identifier>() { Identifier });

        /// <summary>
        /// Selects model objects with the given Identifiers
        /// </summary>
        /// <param name="Identifiers">Tekla object Identifiers list</param>
        public static void Select(List<TS.Identifier> Identifiers) =>
            new TSM.UI.ModelObjectSelector().Select(new ArrayList(GetObjects.Get(Identifiers)));

        /// <summary>
        /// Add model object with given GUID to selected parts in model
        /// </summary>
        /// <param name="Guid">Tekla object GUID</param>
        public static void AddSelect(string Guid) =>
            AddSelect(new TSM.Model().GetIdentifierByGUID(Guid));

        /// <summary>
        /// Add model object with given GUID to selected parts in model
        /// </summary>
        /// <param name="Guids">Tekla object GUIDs list</param>
        public static void AddSelect(List<string> Guids)
        {
            List<TS.Identifier> lst = new List<TS.Identifier>();

            Guids.ForEach(x => lst.Add(new TSM.Model().GetIdentifierByGUID(x)));

            AddSelect(lst);
        }

        /// <summary>
        /// Add model object with given ID to selected parts in model
        /// </summary>
        /// <param name="Id">Tekla object ID</param>
        public static void AddSelect(int Id) =>
            AddSelect(new TS.Identifier(Id));

        /// <summary>
        /// Add model object with given IDs to selected parts in model
        /// </summary>
        /// <param name="Ids">Tekla object IDs list</param>
        public static void AddSelect(List<int> Ids)
        {
            TSM.UI.ModelObjectSelector sel = new TSM.UI.ModelObjectSelector();

            List<TS.Identifier> list = new List<TS.Identifier>();

            Ids.ForEach(x => list.Add(new TS.Identifier(x)));

            AddSelect(list);
        }

        /// <summary>
        /// Add model object with given Identifier to selected parts in model
        /// </summary>
        /// <param name="Identifier">Tekla object Identifier</param>
        public static void AddSelect(TS.Identifier Identifier) =>
            AddSelect(new List<TS.Identifier>() { Identifier });

        /// <summary>
        /// Add model object with given Identifiers to selected parts in model
        /// </summary>
        /// <param name="Identifiers">Tekla object Identifiers list</param>
        public static void AddSelect(List<TS.Identifier> Identifiers)
        {
            TSM.UI.ModelObjectSelector selector = new TSM.UI.ModelObjectSelector();

            var values = selector.GetSelectedObjects();

            ArrayList lst = new ArrayList();

            while(values.MoveNext())
                lst.Add(values.Current);

            lst.AddRange(GetObjects.Get(Identifiers));

            new TSM.UI.ModelObjectSelector().Select(lst);
        }

    }
}
