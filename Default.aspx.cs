using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StackExchange.Redis_Demo
{
    public partial class Default : System.Web.UI.Page
    {
        #region redisVars

        private static string redisServer = ConfigurationManager.AppSettings["RedisServer"];
        private static string redisPwd = ConfigurationManager.AppSettings["Password"];
        private static string connectionString = redisServer; // + ",password=" + redisPwd;
        private static int dbNum = Convert.ToInt16(ConfigurationManager.AppSettings["RedisDbNum"]);
        private static int expirationDays = Convert.ToInt16(ConfigurationManager.AppSettings["ExpirationDays"]);

        private static IDatabase redisDB { get; set; }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static string SetGetData()
        {
            redisDB = ConnectToRedis(redisServer, dbNum);

            StringBuilder sb = new StringBuilder();
            StringMethods(sb);
            ListMethods(sb);
            SetMethods(sb);
            HashMethods(sb);
            SortedSetMethods(sb);
            return sb.ToString();
        }

        // connect to redis server instance provided server and database number
        // database number ranges from 0 - 15
        private static IDatabase ConnectToRedis(string redisServer, int dbNum)
        {
            ConnectionMultiplexer redisConn = ConnectionMultiplexer.Connect(connectionString);
            IDatabase db = redisConn.GetDatabase(dbNum);
            return db;
        }

        public static void StringMethods(StringBuilder sb) 
        {
            string key = "user:1";
            redisDB.KeyDelete(key);

            sb.AppendLine("----------------------STRING METHODS----------------------");
            string serializedVal = "{'name': 'ivan', 'email': 'ivan@ivan.com'}";
            sb.AppendLine("StringSet key = " + key + ", val = " + serializedVal);
            redisDB.StringSet("user:1", serializedVal);

            sb.AppendLine("StringGet for key = " + key);
            sb.AppendLine("\tValue returned = " + redisDB.StringGet(key));
            sb.AppendLine();
        }

        public static void ListMethods(StringBuilder sb)
        {
            string comment1 = "Comment 1";
            string comment2 = "Comment 2";
            string comment3 = "Comment 3";
            string comment4 = "Comment 4";
            string comment5 = "Comment 5";
            string comment6 = "Comment 6";

            string key = "recentComments";
            redisDB.KeyDelete(key);

            sb.AppendLine("----------------------LIST METHODS----------------------");
            sb.AppendLine("Scenario: Keeping track of last 5 comments on a blog.");
            sb.AppendLine("\tListLeftPush for key = " + key + " , val = " + comment1);
            redisDB.ListLeftPush(key, comment1);
            sb.AppendLine("\tListRange from index 0 to 1 for key = " + key);
            sb.AppendLine("\tValue returned = " + PrettifyRedisValueArray(redisDB.ListRange(key, 0, 1)));

            sb.AppendLine();
            sb.AppendLine("\tListLeftPush for key = " + key + " , val = " + comment2);
            redisDB.ListLeftPush(key, comment2);
            sb.AppendLine("\tListRange from index 0 to 2 for key = " + key);
            var t = redisDB.ListRange(key, 0, 2);
            sb.AppendLine("\t\tValue returned = " + PrettifyRedisValueArray(redisDB.ListRange(key, 0, 2)));

            sb.AppendLine();
            sb.AppendLine("\tListRightPush for key = " + key + " , val = " + comment3);
            redisDB.ListRightPush(key, comment3);
            sb.AppendLine("\tListRightPush for key = " + key + " , val = " + comment4);
            redisDB.ListRightPush(key, comment4);
            sb.AppendLine("\tListRange from index 0 to 5 for key = " + key);
            sb.AppendLine("\t\tValue returned = " + PrettifyRedisValueArray (redisDB.ListRange(key, 0, 5)));

            sb.AppendLine();
            sb.AppendLine("\tListRightPush  for key = " + key + " , val = " + comment5);
            redisDB.ListRightPush(key, comment5);
            sb.AppendLine("\tListRightPush  for key = " + key + " , val = " + comment6);
            redisDB.ListRightPush(key, comment6);
            sb.AppendLine("\tListRange from index 0 to 5 for key = " + key);
            sb.AppendLine("\t\tValue returned = " + PrettifyRedisValueArray(redisDB.ListRange(key, 0, 5)));

            sb.AppendLine();
            sb.AppendLine("\tWe want to keep track of last 5 comments:");
            sb.AppendLine("\t\tListTrim from index 0 to 4 for key = " + key);
            redisDB.ListTrim(key, 0, 4);
            sb.AppendLine("\t\tListRange from index 0 to 5 for key = " + key);
            sb.AppendLine("\t\tValue returned = " + PrettifyRedisValueArray(redisDB.ListRange(key, 0, 5)));

            sb.AppendLine();
            sb.AppendLine("NOTE: Redis lists can be very easily implemented as stacks and queues.");
            sb.AppendLine();
        }

        public static void SetMethods(StringBuilder sb)
        {
            string key1 = "post:1:likes";
            redisDB.KeyDelete(key1);

            sb.AppendLine("----------------------SET METHODS----------------------");
            sb.AppendLine("Scenario: Get list of the people that like a particular post or what kind of posts are different people liking");

            sb.AppendLine();
            RedisValue[] likers1 = { "joe", "bob", "mary" };
            sb.AppendLine("\tSetAdd for key = " + key1 + " , val = " + PrettifyRedisValueArray(likers1));
            redisDB.SetAdd(key1, likers1);
            sb.AppendLine("\tSetLength for key = " + key1);
            sb.AppendLine("\t\tValue returned = " + redisDB.SetLength(key1));

            sb.AppendLine();
            sb.AppendLine("\tSetMembers for key = " + key1);
            sb.AppendLine("\t\tValue returned = " + PrettifyRedisValueArray(redisDB.SetMembers(key1)));

            sb.AppendLine("\tSetContains val = 'joe' for key = " + key1) ;
            sb.AppendLine("\t\tValue returned = " + redisDB.SetContains(key1, "joe"));

            sb.AppendLine();
            RedisValue[] likers2 = { "joe", "time"};
            string key2 = "post:2:likes";
            sb.AppendLine("\tSettAdd for key = " + key2 + " , val = " + PrettifyRedisValueArray(likers2));
            redisDB.SetAdd(key2, likers2);

            sb.AppendLine();
            sb.AppendLine("\tSet Operations: ");
            sb.AppendLine("\t\tIntersection of key1 and key2 sets");
            sb.AppendLine("\t\t\tValue returned = " + PrettifyRedisValueArray(redisDB.SetCombine(SetOperation.Intersect, key1, key2)));
            sb.AppendLine("\t\tDifference of key1 and key2 sets");
            sb.AppendLine("\t\t\tValue returned = " + PrettifyRedisValueArray(redisDB.SetCombine(SetOperation.Difference, key1, key2)));
            sb.AppendLine("\t\tUnion of key1 and key2 sets");
            sb.AppendLine("\t\t\tValue returned = " + PrettifyRedisValueArray(redisDB.SetCombine(SetOperation.Union, key1, key2)));

            sb.AppendLine();
            sb.AppendLine("NOTE: Sets are very fast for removing, inserting, and checking for existence of an element");
            sb.AppendLine();
        }

        public static void HashMethods(StringBuilder sb)
        {
            string key = "user:1:h";
            redisDB.KeyDelete(key);

            sb.AppendLine("----------------------HASH METHODS----------------------");
            sb.AppendLine("\tHashSet for key = " + key + " , field1 = ( innerhash_key = name , innerhash_val = 'joe' )");
            redisDB.HashSet(key, "name" , "joe");

            sb.AppendLine();
            sb.AppendLine("\tHashGet for key = " + key + " , field1 = name");
            sb.AppendLine("\t\tValue returned = " + redisDB.HashGet(key, "name"));

            sb.AppendLine();
            sb.AppendLine("\tHashMultipleSet for key = " + key + " , field1 = ( innerhash_key = email , innerhash_val = 'ivan@ivan.com') , field2 = ( innerhash_key = id, innerhash_val = 150)");
            HashEntry[] data = new HashEntry[] { 
                new HashEntry("email", "ivan@ivan.com"),
                new HashEntry("id", 150)
            };
            redisDB.HashSet(key, data);

            sb.AppendLine();
            RedisValue[] fields = {"name", "id"};
            sb.AppendLine("\tHashMultipleGet for key = " + key + " , field1 = 'name' , field2 = 'id'");
            sb.AppendLine("\t\tValue returned = " + PrettifyRedisValueArray(redisDB.HashGet(key, fields)));

            sb.AppendLine();
            sb.AppendLine("\tHashGetAll for key = " + key);
            sb.AppendLine("\t\tValueReturned = " + PrettifyHashEntryArray(redisDB.HashGetAll(key)));

            sb.AppendLine();
            sb.AppendLine("\tHashKeys for key = " + key);
            sb.AppendLine("\t\tValue returned = " + PrettifyRedisValueArray(redisDB.HashKeys(key)));

            sb.AppendLine();
            sb.AppendLine("\tHashValues for key = " + key);
            sb.AppendLine("\t\tValue returned = " + PrettifyRedisValueArray(redisDB.HashValues(key)));

            sb.AppendLine();
            sb.AppendLine("NOTE: Use hashes when you need to access a specific field instead of a whole blob of string, else use string.");
            sb.AppendLine();
        }

        public static void SortedSetMethods (StringBuilder sb)
        {
            string key = "highscore";
            redisDB.KeyDelete(key);

            sb.AppendLine("----------------------SORTED SET METHODS----------------------");
            sb.AppendLine("Scenario: High score board for a game");
            sb.AppendLine();
            sb.AppendLine("\tSortedSetAdd for key = "  + key + " , vals = [score1 = 120 val1 = 'joe' score2 = 100 val2 = 'bob' score3 = 150 val3 = 'mary' score4 = 90 val4 = 'tim']");
            SortedSetEntry[] data = new SortedSetEntry[] {
                new SortedSetEntry("joe", 120),
                new SortedSetEntry("bob", 100 ),
                new SortedSetEntry("mary", 150),
                new SortedSetEntry("tim", 90)
            };
            redisDB.SortedSetAdd(key, data);

            sb.AppendLine();
            sb.AppendLine("\tSortedSetRangeByRank from index 0 to 4 for key = " + key);
            sb.AppendLine("\t\tValue returned = " + PrettifyRedisValueArray(redisDB.SortedSetRangeByRank(key)));

            sb.AppendLine();
            sb.AppendLine("\tTo update Joe's score");
            sb.AppendLine("\t\tSortedSetAdd score= 125 val = 'joe' for key = " + key);
            redisDB.SortedSetAdd(key, "joe", 125);

            sb.AppendLine();
            sb.AppendLine("\tTo see where a member ranks");
            sb.AppendLine("\t\tSortedSetRank for val = 'bob' and key = " + key);
            sb.AppendLine("\t\tValue returned = " + redisDB.SortedSetRank(key, "bob"));
            sb.AppendLine("\t\tSortedSetRank for val = 'tim' and key = " + key);
            sb.AppendLine("\t\tValue returned = " + redisDB.SortedSetRank(key, "tim"));
            sb.AppendLine("\t\tSortedSetRank for val = 'mary' and key = " + key);
            sb.AppendLine("\t\tValue returned = " + redisDB.SortedSetRank(key, "mary"));
            sb.AppendLine();
        }

        public static string PrettifyRedisValueArray(RedisValue[] input)
        {
            return "[ " + string.Join(" , ", input.ToList()) + " ]";
        }

        public static string PrettifyHashEntryArray(HashEntry[] input)
        {
            return "[ " + string.Join(" , ", input.ToList()) + " ]";
        }
    }
}