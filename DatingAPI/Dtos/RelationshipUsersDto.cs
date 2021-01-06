using DatingAPI.Models;
using DatingAPI.Models.Relationship;
using System.Collections.Generic;

namespace DatingAPI.Dtos
{
  public class RelationshipUsersDto
  {
    public List<RelationshipModel> Relationships { get; set; }
    public List<UserModel> Users { get; set; }
  }
}
