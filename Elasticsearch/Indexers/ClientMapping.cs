using Nest;

internal static class ClientMapping
{
    public static ITypeMapping Mapping(TypeMappingDescriptor<Client> mapping) => mapping
        .AutoMap()
        .Properties<Client>(p => p
            .Text(t => t.Name(client => client.Name))
            .Keyword(t => t.Name(client => client.Name))
            .Keyword(t => t.Name(client => client.Level))
            .Object<Sales>(o => o
                .Name(client => client.Sales)
                .Properties(pr => pr
                    .Keyword(k => k.Name(n => n.Lastname))
                    .Keyword(k => k.Name(n => n.Firstname))
                    .Keyword(k => k.Name(n => n.Revenue))
                    .Keyword(k => k.Name(sales => sales.Products))
                )
            )
        );
}