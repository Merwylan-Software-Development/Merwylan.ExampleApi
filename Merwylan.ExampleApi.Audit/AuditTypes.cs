namespace Merwylan.ExampleApi.Audit
{
    public enum AuditTypes
    {
        PostArticle = 1,
        PutArticle = 2,
        PostInbound = 3,
        ProxyInbound = 4,
        PostOutbound = 5,
        ProxyOutbound = 6,
        Receipt = 7,
        Picking = 8,
        OutboundOut = 9,
        Despatch = 10,
        TransactionConf = 11,
        DeleteOutbound = 12,
        StockTransactions = 13,
        SalesOrderlinePickQuantity = 14,
        SalesOrderlineStatus = 15,
        SalesOrderShipment = 16,
        CrossdockShipment
    }
}
