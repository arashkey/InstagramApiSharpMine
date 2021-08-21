using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;

namespace InstagramApiSharp.Converters
{
    internal class InstaRecipientsConverter : IObjectConverter<InstaRecipients, IInstaRecipientsResponse>
    {
        public IInstaRecipientsResponse SourceObject { get; set; }

        public InstaRecipients Convert()
        {
            var recipients = new InstaRecipients
            {
                ExpiresIn = SourceObject.Expires,
                Filtered = SourceObject.Filtered,
                RankToken = SourceObject.RankToken,
                RequestId = SourceObject.RequestId
            };
            if (SourceObject?.RankedRecipients?.Length > 0)
                foreach (var recipient in SourceObject.RankedRecipients)
                {
                    if (recipient == null) continue;
                    var fakeThread = new InstaDirectInboxThread();

                    if (recipient.Thread != null)
                    {
                        var rankedThread = new InstaRankedRecipientThread
                        {
                            Canonical = recipient.Thread.Canonical,
                            Named = recipient.Thread.Named,
                            Pending = recipient.Thread.Pending,
                            ThreadId = recipient.Thread.ThreadId,
                            ThreadTitle = recipient.Thread.ThreadTitle,
                            ThreadType = recipient.Thread.ThreadType,
                            ViewerId = recipient.Thread.ViewerId
                        };
                        fakeThread.ThreadId = recipient.Thread.ThreadId;
                        fakeThread.VieweId = recipient.Thread.ViewerId.ToString();
                        fakeThread.Title = recipient.Thread.ThreadTitle;
                        foreach (var user in recipient.Thread.Users)
                        {
                            rankedThread.Users.Add(ConvertersFabric.Instance.GetUserShortConverter(user).Convert());
                            fakeThread.Users.Add(new InstaUserShortFriendship
                            {
                                FriendshipStatus = new InstaFriendshipShortStatus(),
                                FullName = user.FullName,
                                UserName = user.UserName,
                                IsPrivate = user.IsPrivate,
                                IsVerified = user.IsVerified,
                                Pk = user.Pk,
                                ProfilePicture = user.ProfilePicture,
                                ProfilePictureId = user.ProfilePictureId
                            });
                        }
                        recipients.Threads.Add(rankedThread);
                    }

                    if (recipient.User != null)
                    {
                        var user = ConvertersFabric.Instance.GetUserShortConverter(recipient.User).Convert();
                        recipients.Users.Add(user);
                        fakeThread.ThreadId = "FAKETHREAD" + user.Pk;
                        fakeThread.Title = user.UserName.ToLower();
                        fakeThread.Users.Add(new InstaUserShortFriendship
                        {
                            FriendshipStatus = new InstaFriendshipShortStatus(),
                            FullName = user.FullName,
                            UserName = user.UserName,
                            IsPrivate = user.IsPrivate,
                            IsVerified = user.IsVerified,
                            Pk = user.Pk,
                            ProfilePicture = user.ProfilePicture,
                            ProfilePictureId = user.ProfilePictureId
                        });
                    }
                    recipients.Items.Add(fakeThread);
                }

            return recipients;
        }
    }
}