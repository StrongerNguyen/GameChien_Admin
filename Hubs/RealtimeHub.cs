using FT_Admin.Models;
using Microsoft.AspNet.SignalR;
using System;

namespace FT_Admin.Hubs
{
    public class RealtimeHub : Hub
    {
        IHubContext hub = null;
        public RealtimeHub()

        {
            hub = GlobalHost.ConnectionManager.GetHubContext<RealtimeHub>();
        }
        public void updateTransaction(Guid Id)
        {
            hub.Clients.All.updateTransaction(Id);
        }
        public void updatePlayer(Guid Id)
        {
            hub.Clients.All.updatePlayer(Id);
        }

        public void updateTotalDeposit()
        {
            hub.Clients.All.updateTotalDeposit();
        }
        public void updateTotalWithdraw()
        {
            hub.Clients.All.updateTotalWithdraw();
        }
        public void UpdateBankAccountStatus(int bankAccountId, string type = "info", string message = "", int seconds = 0)
        {
            hub.Clients.All.updateBankAccountStatus(bankAccountId, type, message, seconds);
        }
        public void updateCustomerRequest(Guid customerRequestId, bool isNew = false)
        {
            try
            {
                if (isNew)
                {
                    //using (var db = new BankAPIEntities())
                    //{
                    //    tblCustomerRequest customerRequest = db.tblCustomerRequests.FirstOrDefault(t => t.Id == customerRequestId);
                    //    if (customerRequest.Type) newDeposit();
                    //    else newWithdraw();
                    //}
                }
                hub.Clients.All.updateCustomerRequest(customerRequestId);
                updateCountNotify();
            }
            catch (Exception ex)
            {
                Logging.Log.Error(ex);
            }
        }
        public void updateUserUpdatingCustomerRequest(Guid id, string user)
        {
            hub.Clients.All.updateUserUpdatingCustomerRequest(id, user);
        }
        public void updateCountNotify()
        {
            //using (var db = new BankAPIEntities())
            //{
            //    int customerWithoutGameId = db.tblCustomers.Count(t => string.IsNullOrEmpty(t.GameId) && !string.IsNullOrEmpty(t.PhoneNumber));
            //    int nap = db.tblCustomerRequests.Count(t => t.Type && string.IsNullOrEmpty(t.Status));
            //    int rut = db.tblCustomerRequests.Count(t => !t.Type && string.IsNullOrEmpty(t.Status));

            //    hub.Clients.All.updateCountNotify(customerWithoutGameId, nap, rut);
            //}
        }
        public void updateCountTicket()
        {
            //using (var db = new BankAPIEntities())
            //{
            //    int ticket = db.tblCases.Count(t => t.Status != -1 && t.Status != 1);
            //    hub.Clients.All.updateCountTicket(ticket);
            //}
        }

        public void customerRequestCompleted(Guid Id, string Status, bool Type, bool isActiveAccount, string Note)
        {
            hub.Clients.All.customerRequestCompleted(Id, Status, Type, isActiveAccount, Note);
        }
        public void updateCustomer(Guid customerId)
        {
            hub.Clients.All.updateCustomer(customerId);
            this.updateCountNotify();
        }
        public void newDeposit()
        {
            hub.Clients.All.newDeposit();
        }
        public void newWithdraw()
        {
            hub.Clients.All.newWithdraw();
        }
        public void reloadPage()
        {
            hub.Clients.All.reloadPage();
        }
    }
}