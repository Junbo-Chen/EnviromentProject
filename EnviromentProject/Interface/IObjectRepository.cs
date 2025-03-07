using System;
using System.Collections.Generic;
using EnviromentProject.Model;
using Object = EnviromentProject.Model.Object; // Add this using directive

namespace EnviromentProject.Interface
{
    public interface IObjectRepository
    {
        IEnumerable<Object> GetObjectsByEnvironmentId(Guid environmentId);
        Object GetObjectById(Guid id);
        void InsertObject(Object obj);
        void UpdateObject(Object obj);
        void DeleteObject(Guid id);
    }
}