using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DatingAPI.Models;
using DatingAPI.Models.Authen;
using DatingAPI.MongoHelper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace DatingAPI.Data
{
  public class AuthenticationServices : IAuthenticationServices
  {
    private IMongoCollection<AuthenticationModel> _authenCollection;
    private IMongoCollection<UserModel> _userCollection;

    public AuthenticationServices(IConfiguration _config, IMongoContext mongoContext)
    {
      var _mongoClient = new MongoClient(_config.GetSection("DatingSettings:ConnectionString").Value);
      var _database = _mongoClient.GetDatabase(_config.GetSection("DatingSettings:DatabaseName").Value);
      _authenCollection = _database.GetCollection<AuthenticationModel>(typeof(AuthenticationModel).Name);
      _userCollection = _database.GetCollection<UserModel>(typeof(UserModel).Name);
    }

    public async Task<string> Login(string email, string password)
    {
      FilterDefinition<AuthenticationModel> filter = Builders<AuthenticationModel>.Filter.Eq(m => m.Email, email);
      AuthenticationModel authenModel = await _authenCollection.Find(filter).FirstOrDefaultAsync();

      if (authenModel == null)
      {
        return null;
      }

      if (!VerifyPasswordHash(password, authenModel.PasswordHash, authenModel.PasswordSalt))
      {
        return null;
      }

      return authenModel.UserId.ToString();
    }

    private bool VerifyPasswordHash(string password, string passwordHash, string passwordSalt)
    {
      PasswordModel passwordModel = CreatePasswordHash(password, passwordSalt);

      if (passwordModel.PasswordHash == passwordHash)
      {
        return true;
      }

      return false;
    }

    public async Task<string> Register(UserModel user, string password)
    {
      PasswordModel passwordModel = CreatePasswordHash(password);

      AuthenticationModel authenModel = new AuthenticationModel();
      authenModel.PasswordSalt = passwordModel.PasswordSalt;
      authenModel.PasswordHash = passwordModel.PasswordHash;
      authenModel.Email = user.Email;
      authenModel.UserId = user.ObjectId;

      try
      {
        await _authenCollection.InsertOneAsync(authenModel);
        await _userCollection.InsertOneAsync(user);
        return user.ObjectId.ToString();
      }
      catch
      {
        return null;
      }
    }

    private PasswordModel CreatePasswordHash(string password, string passwordSalt = null)
    {
      byte[] salt = new byte[128 / 8];
      if (passwordSalt == null)
      {
        using (var rng = RandomNumberGenerator.Create())
        {
          rng.GetBytes(salt);
        }
      }
      else
      {
        salt = Convert.FromBase64String(passwordSalt);
      }

      PasswordModel passwordModel = new PasswordModel();
      passwordModel.PasswordSalt = Convert.ToBase64String(salt);
      passwordModel.PasswordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

      return passwordModel;
    }

    public async Task<bool> UserExists(string email)
    {
      AuthenticationModel authenModel = await _authenCollection.Find(x => x.Email == email).FirstOrDefaultAsync();
      if (authenModel == null)
      {
        return false;
      }

      return true;
    }

    public async Task<bool> VerifyEmail(string email)
    {

      return false;
    }
  }

  public class PasswordModel
  {
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
  }
}
