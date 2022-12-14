using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TL;

namespace TGSendReceivePOC
{
    public class Program
    {
        static User My;
        static readonly Dictionary<long, User> Users = new Dictionary<long, User>();
        static readonly Dictionary<long, ChatBase> Chats = new Dictionary<long, ChatBase>();

        static async Task Main(string[] args)
        {
            List<Deals> deals = new List<Deals>
            {
                new Deals { Id = 1, AffiliateUrl = "https://www.flipkart.com/make-ur-wish-2-1-power-full-led-rechargeable-long-range-torch-up-km-backlight-6-hrs-emergency-light/p/itm0527e582811a6?pid=EMLGAUZ3NDFFSQZ6&lid=LSTEMLGAUZ3NDFFSQZ6R8GLU2&marketplace=FLIPKART&store=jhg%2Fyqn&srno=b_1_1&otracker=hp_reco_Shop%2BMonthly%2BEssentials_3_15.dealCard.OMU_cid%3AS_F_N_jhg_yqn__d_50-100__NONE_ALL%3Bnid%3Ajhg_yqn_%3Bet%3AS%3Beid%3Ajhg_yqn_%3Bmp%3AF%3Bct%3Ad%3B_10&otracker1=hp_reco_WHITELISTED_personalisedRecommendation%2FC5_Shop%2BMonthly%2BEssentials_DESKTOP_HORIZONTAL_dealCard_cc_3_NA_view-all_10&fm=personalisedRecommendation%2FC5&iid=en_3nJoRUs25nWvxxEJeUwWxqkqdalHYa8j%2BgKMmSEGCz2X%2F0mWJIC0gQC2lbwzj3l%2F583VH1QfiiqjQzu652Mwwg%3D%3D&ppt=browse&ppn=browse&ssid=nma4v1vas00000001662799984833", DealTitle = "Title 1" },
                new Deals { Id = 2, AffiliateUrl = "https://www.flipkart.com/urban-terrain-ut2000-mtb-27-5-t-mountain-cycle/p/itm733a4be510a52?pid=CCEFZJZCK5QPSSUN&lid=LSTCCEFZJZCK5QPSSUNWH0XZF&marketplace=FLIPKART&store=abc&srno=b_1_2&otracker=hp_omu_Beauty%252C%2BFood%252C%2BToys%2B%2526%2Bmore_1_5.dealCard.OMU_3Q4YN03QMXE9_5&otracker1=hp_omu_PINNED_neo%2Fmerchandising_Beauty%252C%2BFood%252C%2BToys%2B%2526%2Bmore_NA_dealCard_cc_1_NA_view-all_5&fm=organic&iid=en_cJQHGMuHwsr1QV4hx0b0PjxIPMi%2BFrmsAEXpAC0IRwGbwvrIjQHp43xd3C0QiIQ64oXfSLYg09%2Bbue%2FEpVSD6A%3D%3D&ppt=browse&ppn=browse&ssid=jb1mc790nk0000001662799966402", DealTitle = "Title 2" },
                new Deals { Id = 2, AffiliateUrl = "https://www.flipkart.com/epson-l3251-multi-function-wifi-color-printer/p/itmb0bd42a416cb8?pid=PRNG87QKNKBCE36H&lid=LSTPRNG87QKNKBCE36HR7XP4E&marketplace=FLIPKART&store=6bo%2Ftia%2Fffn%2Ft64&srno=b_1_1&otracker=hp_omu_Best%2Bof%2BElectronics_6_3.dealCard.OMU_JW61XPR994XX_3&otracker1=hp_rich_navigation_PINNED_neo%2Fmerchandising_NA_NAV_EXPANDABLE_navigationCard_cc_2_L2_view-all%2Chp_omu_PINNED_neo%2Fmerchandising_Best%2Bof%2BElectronics_NA_dealCard_cc_6_NA_view-all_3&fm=neo%2Fmerchandising&iid=cb4b74da-6aac-4bf6-803f-a23488838bac.PRNG87QKNKBCE36H.SEARCH&ppt=browse&ppn=browse&ssid=k7dp9vd5zk0000001662799965989", DealTitle = "Title 3" }
            };


            using var client = new WTelegram.Client(Config);

            client.OnUpdate += Client_OnUpdate;
            My = await client.LoginUserIfNeeded();
            Users[My.id] = My;
            // Note: on login, Telegram may sends a bunch of updates/messages that happened in the past and were not acknowledged
            Console.WriteLine($"We are logged-in as {My.username ?? My.first_name + " " + My.last_name} (id {My.id})");

            var hash_code = My.GetHashCode();


            var chats = await client.Messages_GetAllChats();
            Console.WriteLine("This user has joined the following:");
            foreach (var (id, chat) in chats.chats)
                switch (chat) // example of downcasting to their real classes:
                {
                    case Chat basicChat when basicChat.IsActive:
                        Console.WriteLine($"{id}:  Basic chat: {basicChat.title} with {basicChat.GetHashCode()} members");
                        break;
                    case Channel group when group.IsGroup:
                        Console.WriteLine($"{id}: Group {group.username}: {group.title}");
                        break;
                    case Channel channel:
                        Console.WriteLine($"{id}: Channel {channel.username}: {channel.title}");
                        break;
                        //case User user:
                        //    Console.WriteLine($"{id}: Channel {user.username}: {user.access_hash}");
                        //    break;
                }

            //var hashIdOfGopaisa = resolved.User.access_hash;
            //var peerId = resolved.peer;

            var dialogs = await client.Messages_GetAllDialogs(); // dialogs = groups/channels/users
            dialogs.CollectUsersChats(Users, Chats);

            var user = dialogs.users.Where(x => x.Key == Constants.PeerId).FirstOrDefault();

            // Send message to channel
            var resolved = await client.Contacts_ResolveUsername("Gopaisa_bot"); // username without the @
            //await client.SendMessageAsync(resolved, "https://www.flipkart.com/dell-s-series-24-inch-full-hd-ips-panel-monitor-s2421hn/p/itm5cbe77f77137a?pid=MONGYDFQTN8ZTEHG&lid=LSTMONGYDFQTN8ZTEHGUGWX0A&marketplace=FLIPKART&store=6bo%2Fg0i%2F9no&srno=b_1_1&otracker=hp_omu_Best%2Bof%2BElectronics_3_3.dealCard.OMU_BK9ZVR1JH3RT_3&otracker1=hp_rich_navigation_PINNED_neo%2Fmerchandising_NA_NAV_EXPANDABLE_navigationCard_cc_3_L2_view-all%2Chp_omu_PINNED_neo%2Fmerchandising_Best%2Bof%2BElectronics_NA_dealCard_cc_3_NA_view-all_3&fm=neo%2Fmerchandising&iid=052e09fd-dac5-43cb-8da5-365747a49b2a.MONGYDFQTN8ZTEHG.SEARCH&ppt=hp&ppn=homepage&ssid=3h1hgd4sow0000001662283644302");

            foreach (var deal in deals)
            {
                await client.SendMessageAsync(resolved, deal.AffiliateUrl);

                await Task.Delay(4000);

                var message = await client.Messages_GetHistory(user.Value, 0, default, 0, 1, 0, 0, 0);

                var test = message.Messages[0];

                Console.WriteLine(((TL.Message)test).message);

                deal.AffiliateUrl = message.Messages[0].ToString();
            }

            

            //for (int offset = 0; ;)
            //{
            //    var messagesBase = await client.Messages_GetHistory(user.Value, 0, default, 0, 1, 0, 0, 0);
            //    if (messagesBase is not Messages_ChannelMessages channelMessages) break;
            //    foreach (var msgBase in channelMessages.messages)
            //        if (msgBase is Message msg)
            //        {
            //            // process the message
            //        }
            //    offset += channelMessages.messages.Length;
            //    if (offset >= channelMessages.count) break;
            //}


            // We collect all infos about the users/chats so that updates can be printed with their names
            //var dialogs = await client.Messages_GetAllDialogs(); // dialogs = groups/channels/users
            //dialogs.CollectUsersChats(Users, Chats);
            Console.ReadKey();
        }

        static string Config(string what)
        {
            switch (what)
            {
                case "api_id": return "9733597";
                case "api_hash": return "3d0fc070b4ff68416d11070b5f497250";
                case "phone_number": return "+919920470755";
                case "verification_code": Console.Write("Code: "); return Console.ReadLine();
                case "first_name": return "John";      // if sign-up is required
                case "last_name": return "Doe";        // if sign-up is required
                case "password": return "secret!";     // if user has enabled 2FA
                default: return null;                  // let WTelegramClient decide the default config
            }
        }

        // if not using async/await, we could just return Task.CompletedTask
        static async Task Client_OnUpdate(IObject arg)
        {
            Console.WriteLine(arg.GetType());
            if (arg is not UpdatesBase updates) return;

            var test = Users.Where(x => x.Key == Convert.ToInt64(8741634995)).ToDictionary(y => y.Key, y => y.Value);

            updates.CollectUsersChats(Users, Chats);
            foreach (var update in updates.UpdateList)
                switch (update)
                {
                    case UpdateNewMessage unm: await DisplayMessage(unm.message); break;
                    //case UpdateEditMessage uem: await DisplayMessage(uem.message, true); break;
                    //// Note: UpdateNewChannelMessage and UpdateEditChannelMessage are also handled by above cases
                    //case UpdateDeleteChannelMessages udcm: Console.WriteLine($"{udcm.messages.Length} message(s) deleted in {Chat(udcm.channel_id)}"); break;
                    //case UpdateDeleteMessages udm: Console.WriteLine($"{udm.messages.Length} message(s) deleted"); break;
                    //case UpdateUserTyping uut: Console.WriteLine($"{User(uut.user_id)} is {uut.action}"); break;
                    //case UpdateChatUserTyping ucut: Console.WriteLine($"{Peer(ucut.from_id)} is {ucut.action} in {Chat(ucut.chat_id)}"); break;
                    //case UpdateChannelUserTyping ucut2: Console.WriteLine($"{Peer(ucut2.from_id)} is {ucut2.action} in {Chat(ucut2.channel_id)}"); break;
                    //case UpdateChatParticipants { participants: ChatParticipants cp }: Console.WriteLine($"{cp.participants.Length} participants in {Chat(cp.chat_id)}"); break;
                    //case UpdateUserStatus uus: Console.WriteLine($"{User(uus.user_id)} is now {uus.status.GetType().Name[10..]}"); break;
                    //case UpdateUserName uun: Console.WriteLine($"{User(uun.user_id)} has changed profile name: @{uun.username} {uun.first_name} {uun.last_name}"); break;
                    //case UpdateUserPhoto uup: Console.WriteLine($"{User(uup.user_id)} has changed profile photo"); break;
                    default: Console.WriteLine(update.GetType().Name); break; // there are much more update types than the above cases
                }
        }

        // in this example method, we're not using async/await, so we just return Task.CompletedTask
        static Task DisplayMessage(MessageBase messageBase, bool edit = false)
        {
            if (edit) Console.Write("(Edit): ");
            switch (messageBase)
            {
                case Message m: Console.WriteLine($"{Peer(m.from_id) ?? m.post_author} in {Peer(m.peer_id)}> {m.message}"); break;
                case MessageService ms: Console.WriteLine($"{Peer(ms.from_id)} in {Peer(ms.peer_id)} [{ms.action.GetType().Name[13..]}]"); break;
            }
            return Task.CompletedTask;
        }

        static string User(long id) => Users.TryGetValue(id, out var user) ? user.ToString() : $"User {id}";
        static string Chat(long id) => Chats.TryGetValue(id, out var chat) ? chat.ToString() : $"Chat {id}";
        static string Peer(Peer peer) => peer is null ? null : peer is PeerUser user ? User(user.user_id)
            : peer is PeerChat or PeerChannel ? Chat(peer.ID) : $"Peer {peer.ID}";
    }
}
