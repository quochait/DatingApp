using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DatingAPI.Models;
using DatingAPI.Models.Authen;
using DatingAPI.Models.Result;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MongoDB.Driver;

namespace DatingAPI.Data
{
  public class AuthenticationServices : IAuthenticationServices
  {
    private IMongoCollection<AuthenticationModel> _authenCollection;
    private IMongoCollection<UserModel> _userCollection;
    private IMemoryCache _memoryCache;
    private IHostingEnvironment _HostEnvironment;
    private IConfiguration configuration;
    private string cacheTokenVerifyEmail = "tokenVerifyEmail";
    private string cacheTokenReset = "tokenResetPassword";
    private string cacheKey = "CachedModel";
    private object existingItem;
    private string templateEmail;

    public AuthenticationServices(IConfiguration _config, IMemoryCache memoryCache, IHostingEnvironment HostEnvironment)
    {
      var _mongoClient = new MongoClient(_config.GetSection("DatingSettings:ConnectionString").Value);
      var _database = _mongoClient.GetDatabase(_config.GetSection("DatingSettings:DatabaseName").Value);
      _authenCollection = _database.GetCollection<AuthenticationModel>(typeof(AuthenticationModel).Name);
      _userCollection = _database.GetCollection<UserModel>(typeof(UserModel).Name);
      configuration = _config;
      _memoryCache = memoryCache;
      _HostEnvironment = HostEnvironment;
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

    public async Task<AuthenticationModel> GetUser(string email)
    {
      AuthenticationModel authenModel = await _authenCollection.Find(x => x.Email == email).FirstOrDefaultAsync();
      return authenModel;
    }
    public bool SendTokenVerifyEmail(string userId, string email)
    {
      string randomString = RandomString(128);
      string hashedRandomString = CreateStringMD5(randomString);

      // add memcached
      CacheModel cache = new CacheModel(userId, cacheTokenVerifyEmail, hashedRandomString);
      UpdateCached(cache);

      string urlToCheck = configuration.GetSection("Hosts:UrlClientVerifyEmail").Value + hashedRandomString;

      using (var mailMessage = new MailMessage())
      {
        using (var client = new SmtpClient("smtp.gmail.com", 587))
        {
          //provide credentials
          string emailFrom = configuration.GetSection("EmailSettings:EmailFrom").Value;
          string password = configuration.GetSection("EmailSettings:Password").Value;

          client.Credentials = new NetworkCredential(emailFrom, password);
          client.EnableSsl = true;

          // configure the mail message
          mailMessage.From = new MailAddress(emailFrom);
          mailMessage.To.Add(new MailAddress(email));
          mailMessage.Subject = "VERIFY EMAIL DATING APPLICATION";
          //mailMessage.Body = string.Format("<!DOCTYPE html><html><head><title>Dating App</title><style type=\"text/css\">body{margin:0 0;padding:0 0}.wrapper{min-height:400px;min-width:400px;width:40%;margin:auto;background-image:linear-gradient(to bottom right, #fd716c, #ffaa4b);border:1px solid white;border-radius:5px 20px 5px}.wrapper{text-align:center;color:white;font-size:bold;font-size:1.5em}.wrapper{}.header{margin-bottom:80px}.content{margin:auto;width:85%;background:#22a822;border-radius:5px;padding:10px 100px;text-decoration:none;color:white}.content:hover{background:#1b841b}.footer{margin-top:90px;color:#550909;font-style:italic}</style></head><body><div class=\"wrapper\"><div class=\"header\"><h1>{0}</h1></div> <a href=\"{1}\" class=\"content\">Click here</a><div class=\"footer\"><p>Email: support@dating.com</p></div></div></body></html>", "Verify email to Dating application", urlToCheck);
          //mailMessage.Body = "Click to verify email DatingApp: " + urlToCheck;

          mailMessage.Body = CreateBody("Click to verify email DatingApp", urlToCheck);
          mailMessage.IsBodyHtml = true;

          //send email
          try
          {
            client.Send(mailMessage);
            return true;
          }
          catch (Exception)
          {
            return false;
          }
        }
      }
    }

    private string CreateBody(string title, string link)
    {
      string contentRootPath = _HostEnvironment.ContentRootPath;
      string emailTemplatePath = Path.Combine(contentRootPath, "Common", "EmailTemplate.html");
      string body = string.Empty;

      using (StreamReader reader = new StreamReader(emailTemplatePath))
      {
        body = reader.ReadToEnd();
      }

      body = body.Replace("{appHeader}", title);
      body = body.Replace("{link}", link);

      return body;
    }

    public bool SendTokenResetPassword(string userId, string email)
    {
      string randomString = RandomString(128);
      string hashedRandomString = CreateStringMD5(randomString);

      // Add cache
      CacheModel cache = new CacheModel(userId, cacheTokenReset, hashedRandomString);
      UpdateCached(cache);

      string urlToCheck = configuration.GetSection("Hosts:UrlClientResetPassword").Value + hashedRandomString;

      using (var mailMessage = new MailMessage())
      {
        using (var client = new SmtpClient("smtp.gmail.com", 587))
        {
          //provide credentials
          string emailFrom = configuration.GetSection("EmailSettings:EmailFrom").Value;
          string password = configuration.GetSection("EmailSettings:Password").Value;

          client.Credentials = new NetworkCredential(emailFrom, password);
          client.EnableSsl = true;

          // configure the mail message
          mailMessage.From = new MailAddress(emailFrom);
          mailMessage.To.Add(new MailAddress(email));
          //mailMessage.Subject = "VERIFY EMAIL DATING APPLICATION";
          mailMessage.Subject = "RESET PASSWORD DATING APPLICATION";
          mailMessage.Body = CreateBody("RESET PASSWORD DATING APPLICATION", urlToCheck);
          mailMessage.IsBodyHtml = true;

          //mailMessage.Body = "Click to reset password DatingApp: " + urlToCheck;

          //send email
          try
          {
            client.Send(mailMessage);
            return true;
          }
          catch (Exception)
          {
            return false;
          }
        }
      }
    }

    private string CreateStringMD5(string input)
    {
      using (MD5 md5 = MD5.Create())
      {
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hashBytes.Length; i++)
        {
          sb.Append(hashBytes[i].ToString("X2"));
        }

        return sb.ToString();
      }
    }

    private string RandomString(int length)
    {
      Random random = new Random();
      const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
      return new string(Enumerable.Repeat(chars, length)
        .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public ResultModel CheckTokenEmail(string userId, string token)
    {
      ResultModel result = new ResultModel();
      
      CacheModel cache = GetCached(userId, cacheTokenVerifyEmail);
      if (cache != null)
      {
        if (cache.Value == token)
        {
          result.True = true;

          FilterDefinition<UserModel> filter = Builders<UserModel>.Filter.Eq(u => u.ObjectId, userId);
          UpdateDefinition<UserModel> update = Builders<UserModel>.Update.Set(u => u.IsVerifyEmail, true);
          var _ = _userCollection.FindOneAndUpdate(filter, update);

          return result;
        }
        else
        {
          result.Error = "Token not matched.";
          return result;
        }
      }
      else
      {
        result.Error = "Token Expiration";
        return result;
      }
    }

    public ResultModel CheckTokenReset(string userId, string token)
    {
      ResultModel result = new ResultModel();
      CacheModel cache = GetCached(userId, cacheTokenReset);
      if (cache != null)
      {
        if (cache.Value == token)
        {
          result.True = true;
          return result;
        }
        else
        {
          result.Error = "Token not matched.";
          return result;
        }
      }
      else
      {
        result.Error = "Token Expiration";
      }

      return result;
    }

    public async Task<bool> UpdatePassword(string userId, string newPassword)
    {
      PasswordModel passwordModel = CreatePasswordHash(newPassword);
      FilterDefinition<AuthenticationModel> filter = Builders<AuthenticationModel>.Filter.Eq(u => u.UserId, userId);
      AuthenticationModel authenModel = await _authenCollection.Find(filter).FirstOrDefaultAsync();
      if (authenModel != null)
      {
        UpdateDefinition<AuthenticationModel> update = Builders<AuthenticationModel>.Update.
          Set(au => au.PasswordHash, passwordModel.PasswordHash).
          Set(au => au.PasswordSalt, passwordModel.PasswordSalt);
        await _authenCollection.FindOneAndUpdateAsync(filter, update);
        return true;
      }

      return false;
    }

    private bool UpdateCached(CacheModel cacheUpdate)
    {
      CacheModel cache = new CacheModel();
      List<CacheModel> caches = new List<CacheModel>();

      if (_memoryCache.TryGetValue<List<CacheModel>>("CachedModel", out caches))
      {
        //cache = caches.Where(x => x.UserId == cacheUpdate.UserId && x.Name == cacheUpdate.Name).FirstOrDefault();
        caches.Where(x => x.UserId == cacheUpdate.UserId && x.Name == cacheUpdate.Name).ToList().ForEach(c => c.Value = cacheUpdate.Value);
      }
      else
      {
        cache = cacheUpdate;
        caches = new List<CacheModel>();
        caches.Add(cache);
      }

      _memoryCache.Set(cacheKey, caches.ToList(), new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

      return true;
    }

    private CacheModel GetCached(string userId, string name)
    {
      List<CacheModel> caches = new List<CacheModel>();

      try
      {
        caches = _memoryCache.Get("CachedModel") as List<CacheModel>;
      }
      catch (Exception)
      {
        return null;
      }

      try
      {
        CacheModel cache = caches.Where(x => x.UserId == userId && x.Name == name).FirstOrDefault();
        //caches.Remove(cache);
        //_memoryCache.Set(cacheKey, caches.ToList(), new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));
        return cache;
      }
      catch (Exception)
      {
        return null;
      }
    }

    private void RemoveCached(string userId, string name)
    {
      List<CacheModel> caches = new List<CacheModel>();
      caches.RemoveAll(x => x.UserId == userId && x.Name == name);
    }
  }

  public class PasswordModel
  {
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
  }
}
