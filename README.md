# AnimeTrackingBots

This repository contains multiple programs which track anime releases from Crunchyroll, FUNimation, DAISUKI and Taiga.
New releases will be posted to reddit and saved in the database to prevent double posting.

If something is unclear do not hesitate to email me at Shadoxfix@gmail.com or make an issue on this repository.

Refer to database.sqlite for an example database. The example database was used on October 31st 2015.

The programs should be self explanatory but here's a quick summary:  
Create a database and enter data for shows that have to get tracked. I **strongly recommend** opening the provided
example database (using something like "DB Browser for SQLite") to get a feel for what data is needed.
After that run one or multiple bots and start them.

The Crunchyroll, FUNimation and DAISUKI bots have a GUI which should help you out. If you intend to use the program outside of the US
you will have to specify a proxy for FUNimation otherwise you'll get a CloudFlare page which prevents access to the API.
The DAISUKI API returns different values per region so it also has proxy support. The Crunchyroll RSS feeds are always available and as a result has
no proxy support.

Useful API links:

* FUNimation 1: http://funimation.com/feeds/ps/shows?limit=100000 for a list of all shows
* FUNimation 2: http://funimation.com/feeds/ps/videos?ut=FunimationSubscriptionUser&show_id=SHOW_ID_FROM_PREVIOUS_LINK for data on a specific show
* Crunchyroll 1: http://crunchyroll.com/SHOW_DIRECTORY.rss (http://crunchyroll.com/the-kawai-complex-guide-to-manors-and-hostel-behavior.rss for example).
* Crunchyroll 2: Sometimes shows do not have a rss feed or an outdated one (18+ shows and shows with multiple seasons for example). Use http://crunchyroll.com/rss/anime instead.
Be aware that it is delayed by a few minutes and should only be used as a last resort since it contains much more information than you usually need for one show causing the bot to take longer.
* DAISUKI 1: https://www.daisuki.net/api2/search/mode:1 for a list of all shows in the region. (Be aware that the "www." is necessary.)
* DAISUKI 2: https://www.daisuki.net/api2/seriesdetail/SHOW_ID for data on a specific show. (Be aware that the "www." is necessary.)

The Taiga bot relies on two arguments being given when run. These arguments are given by a modified version of Taiga found [here](https://github.com/Shadoxfix/taiga).
There is a [feature request](https://github.com/erengy/taiga/issues/95) to make this a default option but is currently not implemented.
Also note that the Taiga bot needs a file called "data.txt" in the same location as the executable which contains the path to the database file on the first line and the
subreddit to post to on the second line.

What are the episode offsets in the database?
Sometimes sites use different numbering schemes than desired and the episode offsets help you deal with them.
If a show is on episode 13 because a site continues its numbering and you'd like to start over from episode 1
you'd set InternalOffset to -12.0 for that specific entry. If you still want to display a message that it is also known
as episode 13 you'd also enter 12.0 in the AKAOffset cell.

Known issues:

* You will frequently encounter "Failed connect" errors when the time is on exactly an hour. This is a .NET SChannel bug reported [here](https://github.com/dotnet/corefx/issues/3889).
These errors can safely be ignored since the bot will try again a little later.
* After a few hours of posting inactivity the next post on reddit will give you a "Failed reddit post" error. I am not sure as to why this happens but it might be related to the same
bug as mentioned earlier. Using OAuth2 instead of plain username + password does not resolve this. This error can safely be ignored since the bot will try again a little later.
* When stopping the bot the next start attempt may give you a "Failed reddit login" error. Once again, this can safely be ignored. Pressing start again will work fine.

Pro tip:  
If you need to force close the bot hold shift before attempting to do so. It will override the safety measures put in place. Keep in mind that recent data may be corrupted
as a result and should be double checked.
