using System;
using System.Activities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System.Linq;
using static apuk_IOrganizationServiceContext;
using Microsoft.Xrm.Sdk.Client;
using static apuk_GenericDataAccess;

public class CalculateAllLeads : CodeActivity
{
    protected override void Execute(CodeActivityContext Workflowcontext)
    {
        System.Threading.Thread.Sleep(1000);
        IWorkflowContext context = Workflowcontext.GetExtension<IWorkflowContext>();
        IOrganizationServiceFactory serviceFactory = Workflowcontext.GetExtension<IOrganizationServiceFactory>();
        ITracingService tracingService = Workflowcontext.GetExtension<ITracingService>();
        IOrganizationServiceContext service = Workflowcontext.GetExtension<IOrganizationServiceContext>();

        GetAllLeads getAllLeads = new GetAllLeads(service);
        var leadRecords = getAllLeads.GetAllLeadsForCalculate();

        tracingService.Trace($"Count of contract records for calculation is: {leadRecords.Count}");
        
        foreach (var leadRecord in leadRecords)
        {
            try
            {
                tracingService.Trace($"Start calculating contract with lead id='{leadRecord.Id}'");


                CalculateOneLead.Calculate(Workflowcontext, leadRecord);
                tracingService.Trace($"Finish calculating contract with id='{leadRecord.Id}'");
            }
            catch (Exception ex)
            {
                tracingService.Trace($"Error occurred during AX Contract calculation with id='{leadRecord.Id}. Error: {ex.Message}, {ex.StackTrace}'");
            }
        }
    }

    
   }
 public  class GetAllLeads: GenericDataAccess<Lead>
{
    public GetAllLeads(IOrganizationServiceContext service) : base(service)
    {
    }

    public  IList<Lead> GetAllLeadsForCalculate()
    {
        return (from lead in GetAll()//where lead.statecode != null && contract.statecode.Value == ap_axcontractState.Active
                select lead).ToList();
    }
}
public class GenericDataAccess<T> : IGenericDataAccess<T>
       where T : Entity
{
    protected IOrganizationServiceContext service;
    protected GenericDataAccess(IOrganizationServiceContext service)
    {
        this.service = service;
    }

    public virtual IQueryable<T> GetAll()
    {
        return service.CreateQuery<T>();
    }

    public T GetById(Guid id)
    {
        throw new NotImplementedException();
    }

  
}

