using System;
using System.Activities;
using System.Collections.ObjectModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;

public class CalculateOneLead : CodeActivity
{
    private const string LeadEntity = "lead";
    [Input("Lead")]
    [Output("Lead out")]
    [ReferenceTarget(LeadEntity)]
    public InOutArgument<EntityReference> Lead { get; set; }

    enum MarketingPermissionaccess { DoNotAllow = 185530002 };
    protected override void Execute(CodeActivityContext Workflowcontext)
    {

        EntityReference LeadEnt = Lead.Get(Workflowcontext);
        if (LeadEnt == null)
        {
            throw new InvalidOperationException("LeadEntity has not been specified", new ArgumentNullException("LeadEntity"));
        }
        else if (LeadEnt.LogicalName != LeadEntity)
        {
            throw new InvalidOperationException("LeadEntity must reference a LeadEntity ",
                new ArgumentException("LeadEntity must be of type Lead", "LeadEntity"));
        }

        ITracingService tracingService = Workflowcontext.GetExtension<ITracingService>();
        IWorkflowContext context = Workflowcontext.GetExtension<IWorkflowContext>();
        IOrganizationServiceFactory serviceFactory = Workflowcontext.GetExtension<IOrganizationServiceFactory>();
        IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);


        Lead new_lead;

        {
          
            RetrieveRequest request = new RetrieveRequest();
            request.ColumnSet = new ColumnSet(new string[] { "apuk_MarketingPermission", "DoNotPhone", "apuk_donotsms", "DoNotEMail", "DoNotPostalMail" });
            request.Target = LeadEnt;

           
            RetrieveResponse retrieveResponse = (RetrieveResponse)service.Execute(request);
            
            new_lead = retrieveResponse.Entity as Lead;
        }

        if (context.Depth == 1)
        {
            if ((int)new_lead.apuk_MarketingPermission.Value == (int)MarketingPermissionaccess.DoNotAllow)
            {

                new_lead.apuk_donotsms = false;
                new_lead.DoNotPhone = true;
                new_lead.DoNotEMail = true;
                new_lead.DoNotPostalMail = true;
            }
            if (new_lead.apuk_donotsms == false && new_lead.DoNotPhone == true &&
                new_lead.DoNotEMail == true && new_lead.DoNotPostalMail == true)
            {
                new_lead.apuk_MarketingPermission = new OptionSetValue((int)MarketingPermissionaccess.DoNotAllow);
            }
            service.Update(new_lead);




        }
    }
}
