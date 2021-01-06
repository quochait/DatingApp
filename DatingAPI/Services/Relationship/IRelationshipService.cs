﻿using DatingAPI.Models.Relationship;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingAPI.Services.Relationship
{
  public interface IRelationshipService
  {
    Task<List<RelationshipModel>> GetRequestsPending(string userId);
    Task<bool> UpdateRequestStatus(string userId, string fromUserId, string status);
    Task<RelationshipModel> CheckHaveRequest(string userId, string toUserId);
    Task<bool> RemoveRelationship(string userId,string toUserId);
  }
}
