## Redis Demo Code Snippets ##

####Description####
A small demo implementing redis strings, lists, sets, sorted sets, hashes using the StackExchange.Redis c# client. 

####Sync and Async Methods###
Every method implemented is the sync version of the method. Every method has a corressponding async version as well.

####Expiration Keys####
To implement key expiration, please pass the expiration time as the third parameter when you set a key and a value. The expiration time can be days, minutes, seconds, etc.

> Example: redisDB.StringSet(key, valToSave, TimeSpan.FromDays(expirationDays))

####Further Information####
Go to the official documentation for [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis).

