using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using System;

namespace InstagramApiSharp.Converters
{
    internal class InstaUserShortConverter : IObjectConverter<InstaUserShort, InstaUserShortResponse>
    {
        public InstaUserShortResponse SourceObject { get; set; }

        public InstaUserShort Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var user = new InstaUserShort
            {
                Pk = SourceObject.Pk,
                UserName = SourceObject.UserName,
                FullName = SourceObject.FullName,
                IsPrivate = SourceObject.IsPrivate,
                ProfilePicture = SourceObject.ProfilePicture,
                ProfilePictureId = SourceObject.ProfilePictureId,
                IsVerified = SourceObject.IsVerified,
                ProfilePicUrl = SourceObject.ProfilePicture,
                HasAnonymousProfilePicture = SourceObject.HasAnonymousProfilePicture ?? false,
                IsBestie = SourceObject.IsBestie ?? false,
                LatestReelMedia = SourceObject.LatestReelMedia ?? 0
            };
            return user;
        }
    }
}