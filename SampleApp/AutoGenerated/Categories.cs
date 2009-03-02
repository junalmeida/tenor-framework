
using System.Collections.Generic;
using System.Configuration;
using System;

using Tenor.BLL;
using Tenor.Data;

namespace SampleApp.Business.Entities
{
/// <summary>
/// Represents the table Categories.
/// This is a category
/// </summary>
[Serializable(), Table("Categories", "dbo")]
public partial class Categories : BLLBase
{

#region Properties


/// <summary>
/// Represents the field CategoryId.
/// 
/// </summary>
[Field(PrimaryKey=trueAutoKey=true)]
public int CategoryId
{
get
{
return (returnType)GetPropertyValue();
}
set
{
				SetPropertyValue(value);
}
}


/// <summary>
/// Represents the field Name.
/// The name of the category.
/// </summary>
[Field()]
public string Name
{
get
{
return (returnType)GetPropertyValue();
}
set
{
				SetPropertyValue(value);
}
}


/// <summary>
/// Keeps a list of constants with property names.
/// </summary>
public partial class Properties : object {
private Properties() { }

public const string CategoryId = "CategoryId";


public const string Name = "Name";


}


#endregion
#region Foreign Keys


/// <summary>
/// Represents the relationship FK_Items_Categories.
/// </summary>
[ForeignKey(CategoryId, CategoryId)]

public BLLCollection<Items> Itemss
{
get
{
return (BLLCollection<Items>)GetPropertyValue();
}
}


#endregion
#region Constructors And Metadata


public Categories()
{ }


public Categories(bool lazyLoadingDisabled) :
base(lazyLoadingDisabled)
{ }


/// <summary>
/// Loads Categories from the database with these keys.
/// </summary><%
public Categories(int CategoryId) :
base()
{
         this.CategoryId = pCategoryId;

Bind();
}


#endregion
#region Search


#endregion
}
}

