using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using src.models;

namespace src.Controllers
{
    public class ReferalResponse
    {
        public Referal Referal { get; set; }
        public RewardAttribute RewardAttribute { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class RewardsController : ControllerBase
    {
        readonly ILogger _logger;
        readonly IDbService _db;
        readonly IConfiguration _config;

        public RewardsController(ILogger<RewardsController> logger, IDbService db, IConfiguration config)
        {
            _logger = logger;
            _db = db;
            _config = config;
        }

        [HttpGet("{rewardlink}")]
        public async Task<RewardAttribute> GetRewardAttributes(string rewardlink)
        {
            var rewardAttributeCollection = _db.getCollection<RewardAttribute>();
            return await rewardAttributeCollection.Find(r => r.RewardLink == System.Web.HttpUtility.UrlDecode(rewardlink)).FirstAsync();
        }

        [HttpGet("{walletaddress}")]
        public async Task<RewardAttribute[]> GetAllRewardAttributedByWalletAddress(string walletaddress)
        {
            var rewardAttributeCollection = _db.getCollection<RewardAttribute>();
            var rewards = await rewardAttributeCollection.Find(r => r.WalletAddress == walletaddress).ToListAsync();
            return rewards.ToArray();
        }

        [HttpPost]
        public async Task<RewardAttribute> CreateRewardAttributes([FromBody]RewardAttribute rewardAttribute)
        {
            var rewardAttributeCollection = _db.getCollection<RewardAttribute>();
            await rewardAttributeCollection.InsertOneAsync(rewardAttribute);
            return await GetRewardAttributes(rewardAttribute.RewardLink);
        }

        [HttpGet("{personallink}")]
        public async Task<ReferalResponse> GetReferalInfo(string personallink)
        {
            var referalCollection = _db.getCollection<Referal>();
            var rewardAttributeCollection = _db.getCollection<RewardAttribute>();

            var referalResponse = new ReferalResponse();
            var referal = await referalCollection.Find(r => r.PersonalLink == System.Web.HttpUtility.UrlDecode(personallink)).FirstAsync();
            if(referal != null)
            {
                var rewardAttribute = await GetRewardAttributes(referal.RewardLink);
                referalResponse.RewardAttribute = rewardAttribute;

                if (referal.HasClaimed == false && (referal.AmountToClaim/rewardAttribute.AmountPaidPerClick) <= rewardAttribute.MaxPaidClicksPerUser)
                {
                    referal.AmountToClaim = referal.AmountToClaim + rewardAttribute.AmountPaidPerClick;
                    var updated = await referalCollection.UpdateOneAsync(r => r.Id == referal.Id,
                        Builders<Referal>.Update.Set(u => u.AmountToClaim, referal.AmountToClaim));

                    if (updated.MatchedCount != 1)
                    {
                        //probably we have JWT error
                        throw new Exception("failed to update referal");
                    }

                }

                referalResponse.Referal = referal;
            }

            return referalResponse;
        }

        [HttpGet("claim/{personallink}/{walletaddress}")]
        public async Task<ReferalResponse> ClaimReward(string personallink, string walletaddress)
        {
            if(String.IsNullOrWhiteSpace(personallink))
            {
                throw new Exception("personal link can not be empty");
            }

            if(String.IsNullOrWhiteSpace(walletaddress))
            {
                throw new Exception("Wallet address cannot be empty");
            }

            var referalCollection = _db.getCollection<Referal>();
            var referal = await referalCollection.Find(r => r.PersonalLink == System.Web.HttpUtility.UrlDecode(personallink) && r.WalletAddress == walletaddress).FirstAsync();
            
            if (referal == null)
            {
                throw new Exception("Referal not found");
            }

            var updated = await referalCollection.UpdateOneAsync(r => r.Id == referal.Id,
               Builders<Referal>.Update.Set(u => u.HasClaimed, true));

            if (updated.MatchedCount != 1)
            {
                //probably we have JWT error
                throw new Exception("failed to update referal");
            }

            return await GetReferal(personallink);
        }

        [HttpPost("referal")]
        public async Task<ReferalResponse> CreateReferal([FromBody]Referal referal)
        {
            var referalCollection = _db.getCollection<Referal>();
            await referalCollection.InsertOneAsync(referal);

            var ounce = referal.Id;
            var referalLink = "http://frndz.io/" + ounce;

            var updated = await referalCollection.UpdateOneAsync(r => r.Id == referal.Id,
               Builders<Referal>.Update.Set(u => u.PersonalLink, referalLink));

            if (updated.MatchedCount != 1)
            {
                //probably we have JWT error
                throw new Exception("failed to update referal");
            }

            return await GetReferal(referalLink);
        }


        [HttpGet("referal/{walletaddress}")]
        public async Task<Referal[]> GetReferalInfoByWalletAddress(string walletaddress)
        {
            var referalCollection = _db.getCollection<Referal>();
            var referals = await referalCollection.Find(r => r.WalletAddress == walletaddress).ToListAsync();
            return referals.ToArray();
        }

        private async Task<ReferalResponse> GetReferal(string personallink)
        {
            var referalCollection = _db.getCollection<Referal>();
            var rewardAttributeCollection = _db.getCollection<RewardAttribute>();

            var referalResponse = new ReferalResponse();
            var referal = await referalCollection.Find(r => r.PersonalLink == System.Web.HttpUtility.UrlDecode(personallink)).FirstAsync();
            if (referal != null)
            {
                referalResponse.Referal = referal;

                var rewardAttribute = await GetRewardAttributes(referal.RewardLink);
                referalResponse.RewardAttribute = rewardAttribute;
            }

            return referalResponse;
        }
    }
}
