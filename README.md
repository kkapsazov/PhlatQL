# PhlatQL
Ligthwaight mapping library for ASP.NET Core

Available at NuGet: https://www.nuget.org/packages/PhlatQL.Core/


Example usage:
```C#

//DB Models
public class Author
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Book
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public int UserId { get; set; }
    public int AuthorId { get; set; }

    public virtual Author Author { get; set; }
}
```

Get
---
```C#
//PhlatQL types
public class AuthorType : ObjectPhlatType<Author>
{
    public AuthorType()
    {
        this.Field(c => c.Name);
    }
}

public class BookType : ObjectPhlatType<Book>
{
    public BookType()
    {
        this.Field(c => c.Name);
        this.Field<AuthorType>(c => c.Source.Author);
    }
}

//Simple get all controller action
[HttpGet]
public IActionResult All()
{
    List<Book> books = this.dbContext.Books.Include(x => x.Author).ToList();

    return this.Ok(new BookType().Build(books));
}
```

Create
-----
```C#
//PhlatQL create type
public class BookCreateInput<T> : CreatePhlatType<T> where T : Book
{
    //Needed because of JsonPatchDocument
    public BookCreateInput(List<Operation<T>> operations, IContractResolver contractResolver)
        : base(operations, contractResolver)
    {
    }

    //Configure validations if needed
    protected override void Configure()
    {
        this.Field(x => x.AuthorId, new ValidationRuleBuilder().MinValue(1).MaxValue(int.MaxValue).Build());
        this.Field(x => x.UserId, new ValidationRuleBuilder().MinValue(1).MaxValue(int.MaxValue).Build());
        this.Field(x => x.Name, new ValidationRuleBuilder().Regex("^[a-zA-Z ]+$").Required().Build());
    }
}

//Simple create controller action
[HttpPost]
public IActionResult Create([FromBody] BookCreateInput<Book> patch)
{
    Book entity = new();
    patch.ApplyTo(entity);

    ErrorResponse errors = patch.Validate(entity);
    if (errors.Any())
    {
        return this.BadRequest(errors);
    }

    this.dbContext.Books.Add(entity);
    this.dbContext.SaveChanges();

    return this.Ok(new BookType().Build(entity));
}
```

Update
------
```C#
//PhlatQL update type
public class BookUpdateInput<T> : UpdatePhlatType<T> where T : Book
{
    //Needed because of JsonPatchDocument
    public BookInput(List<Operation<T>> operations, IContractResolver contractResolver)
        : base(operations, contractResolver)
    {
    }

    //Configure validations if needed
    protected override void Configure()
    {
        this.Field(x => x.AuthorId, new ValidationRuleBuilder().MinValue(1).MaxValue(int.MaxValue).Build());
        this.Field(x => x.UserId, new ValidationRuleBuilder().MinValue(1).MaxValue(int.MaxValue).Build());
        this.Field(x => x.Name, new ValidationRuleBuilder().Regex("^[a-zA-Z ]+$").Required().Build());
    }
}

//Simple partial update controller action
[HttpPatch]
public IActionResult Update([FromQuery] int id, [FromBody] BookUpdateInput<Book> patch)
{
    Book entity = this.dbContext.Books.Include(x => x.Author).FirstOrDefault(x => x.Id == id);
    if (entity is null)
    {
        return this.NotFound();
    }

    ErrorResponse errors = patch.Validate(entity);
    if (errors.Any())
    {
        return this.BadRequest(errors);
    }

    patch.ApplyTo(entity);

    this.dbContext.Update(entity);
    this.dbContext.SaveChanges();

    return this.Ok(new BookType().Build(entity));
}    

```
