using TSM = Tekla.Structures.Model;
using TS = Tekla.Structures;

namespace Tekla.Structures.Model
{
    public static class GetObject
    {
        /// <summary>
        /// Gets a model object by Guid
        /// </summary>
        /// <param name="Guid">Tekla object GUID</param>
        /// <returns>ModelObject or null if invalid id</returns>
        public static TSM.ModelObject Get(string Guid) =>
            Get(new TSM.Model().GetIdentifierByGUID(Guid));

        /// <summary>
        /// Gets a model object by int ID
        /// </summary>
        /// <param name="Id">Tekla object int ID</param>
        /// <returns>ModelObject or null if invalid id</returns>
        public static TSM.ModelObject Get(int Id) =>
            Get(new TS.Identifier(Id));

        /// <summary>
        /// Gets a model object by tekla identifier
        /// </summary>
        /// <param name="Identifier">Tekla object identifier</param>
        /// <returns>ModelObject or null if invalid identifier</returns>
        public static TSM.ModelObject Get(TS.Identifier Identifier) =>
            new TSM.Model().SelectModelObject(Identifier);

    }
}
