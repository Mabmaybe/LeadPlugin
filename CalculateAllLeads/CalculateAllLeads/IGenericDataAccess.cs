using System;
using System.Linq;
using Microsoft.Xrm.Sdk;

public class apuk_GenericDataAccess
{
    public interface IGenericDataAccess<T> where T : Entity
    {
        T GetById(Guid id);
        IQueryable<T> GetAll();
    }
}
