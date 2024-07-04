Demonstrates an issue (either a bug or with my configuration) with a base class that is used only as an owned relationship but an inherited class is not.

Reported issue: https://github.com/dotnet/efcore/issues/34161

## To reproduce

1. Modify the connection string to match your environment
2. Run `dotnet ef migrations add Moo`
3. Run `dotnet ef migrations add Moo2`
4. Run `dotnet ef database update`

## Observed behaviour

The `Moo2` migration includes a line to drop a non-existent `Discriminator` column:

```
 migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "PersonAddress");
```

The `database update` therefore fails as there's no column to drop.

## Troubleshooting

### Move the AddressId key to the PersonAddress class

`Address` doesn't need the key and the EF migration doesn't actually create it. But when I try this, I get the following when generating any migration:

```
Unable to create a 'DbContext' of type ''. The exception 'The entity type 'PersonAddress' requires a primary key to be defined.
If you intended to use a keyless entity type, call 'HasNoKey' in 'OnModelCreating'. For more information on keyless entity types,
see https://go.microsoft.com/fwlink/?linkid=2141943.'
```

### Change Person.Addresses to be an owned relationship

That is, add this to `OnModelCreating`:

```
modelBuilder.Entity<Person>()
    .OwnsMany(p => p.Addresses);
```

This generates a migration that doesn't compile. The model snapshot and migration designer both have this code:

```
b1.HasDiscriminator().HasValue("PersonAddress");
```

as part of the OwnsMany for Person and `HasDiscriminator` doesn't exist in this context.

### Explicitly define a table per concrete type strategy

```
modelBuilder.Entity<PersonAddress>().UseTpcMappingStrategy
```

No change in behaviour; it's the same as the original issue described above.
