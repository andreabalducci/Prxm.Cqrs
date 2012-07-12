namespace Sample.Domain.Purchases.Domain.Events
{
    public class SupplierChanged
    {
        public string CompanyName { get; protected set; }
        public string Address { get; protected set; }

        public SupplierChanged(string companyName, string address)
        {
            this.CompanyName = companyName;
            this.Address = address;
        }
    }
}