using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

public class apuk_IOrganizationServiceContext
{
    public interface IOrganizationServiceContext : IDisposable
    {
        void AddObject(Entity entity);

        void DeleteObject(Entity entity);

        IQueryable<T> CreateQuery<T>() where T : Entity;

        SaveChangesResultCollection SaveChanges(SaveChangesOptions options);

        OrganizationResponse Execute(OrganizationRequest request);

        void UpdateObject(Entity entity);

        void DeleteLink(Entity source, Relationship relationship, Entity target);

        void AddLink(Entity source, Relationship relationship, Entity target);

        void AttachLink(Entity source, Relationship relationship, Entity target);

        bool IsAttached(Entity entity);

        bool IsAttached(Entity source, Relationship relationship, Entity target);

        void Attach(Entity entity);

        bool Detach(Entity entity);

        Entity Retrieve(string entityLogicalName, Guid id);

        string GetOptionSetTextFromValue(string optionSetValue, string optionSetLogicalName, string entityLogicalName);

        void ClearChanges();
    }
}
