using System;
using System.Activities;
using System.Collections.ObjectModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;

public class CalculateOneLead : CodeActivity
{
    [Input("LeadId")]
    public InArgument<int> LeadId { get; set; }
    [Output("Lead")]
    [ReferenceTarget("lead")]
    public OutArgument<EntityReference> Lead { get; set; }
    enum MarketingPermissionaccess { DoNotAllow = 185530002 };
    protected override void Execute(CodeActivityContext Workflowcontext)
    {
        ITracingService tracingService = Workflowcontext.GetExtension<ITracingService>();
        IWorkflowContext context = Workflowcontext.GetExtension<IWorkflowContext>();
        IOrganizationServiceFactory serviceFactory = Workflowcontext.GetExtension<IOrganizationServiceFactory>();
        IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

        //Use the service reference to call web methods.
        //service.Execute(тут что-то должно быть);


        Guid leadId = this.Lead.Get(Workflowcontext).Id;


        QueryByAttribute query = new QueryByAttribute();
        query.ColumnSet = new ColumnSet("apuk_MarketingPermission", "DoNotPhone", "apuk_donotsms", "DoNotEMail", "DoNotPostalMail");
        query.Attributes.AddRange("apuk_MarketingPermission");
        query.EntityName = this.Lead.Get(Workflowcontext).LogicalName;
        query.Values.AddRange(new object[] { leadId });

        RetrieveMultipleRequest request = new RetrieveMultipleRequest();
        request.Query = query;
        Collection<Entity> entityList = ((RetrieveMultipleResponse)service.Execute(request)).EntityCollection.Entities;
        if (entityList != null)
        {
            Entity entity = (Entity)((RetrieveResponse)service.Execute(request)).Entity;
            Lead lead = (Lead)entity.ToEntity<Lead>();

            if ((int)lead.apuk_MarketingPermission.Value == (int)MarketingPermissionaccess.DoNotAllow)
            {

                lead.apuk_donotsms = false;
                lead.DoNotPhone = true;
                lead.DoNotEMail = true;
                lead.DoNotPostalMail = true;
            }
            if (lead.apuk_donotsms == false && lead.DoNotPhone == true &&
                lead.DoNotEMail == true && lead.DoNotPostalMail == true)
            {
                lead.apuk_MarketingPermission = new OptionSetValue((int)MarketingPermissionaccess.DoNotAllow);
            }
            service.Update(lead);

        }
        else
        {
            return;
        }



    }
}
