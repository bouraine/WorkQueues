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
                    .Text(k => k
                        .Name(n => n.Lastname)
                        .Fields(f => f
                            .Keyword(fk => fk.Name(n => n.Lastname))
                            .Text(fk => fk
                                .Name("lastname_txt")
                                .Analyzer("standard")
                            )
                        )
                    )
                    .Keyword(k => k.Name(n => n.Firstname))
                    .Number(k => k.Name(n => n.Revenue))
                    .Keyword(k => k.Name(sales => sales.Products))
                )
            )
        );
}