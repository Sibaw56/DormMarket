using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

[BsonIgnoreExtraElements]
public class ownerUser
{
    [BsonId]
    public int Id { get; set; }

    [BsonElement("full_Name")]
    public string FullName { get; set; }

    [BsonElement("email")]
    public string Email { get; set; }

    [BsonElement("password")]
    public string Password { get; set; }

    [BsonElement("phone")]
    public string ContactNumber { get; set; }

    [BsonElement("date_Joined")]
    public DateTime DateJoined { get; set; }

    [BsonElement("business_Permit_Path")]
    public string BusinessPermitPath { get; set; }

    [BsonElement("government_Id_Path")]
    public string GovernmentIdPath { get; set; }

    [BsonElement("property_Ownership_Path")]
    public string PropertyOwnershipPath { get; set; }

    [BsonElement("lease_Authorization_Path")]
    public string LeaseAuthorizationPath { get; set; }

    [BsonElement("reviewer_Notes")]
    public string ReviewerNotes { get; set; }

    [BsonElement("is_Verified")]
    public bool IsVerified { get; set; } = false;

    [BsonElement("address")]
    public string Address { get; set; }

    [BsonElement("messenger_Link")]
    public string MessengerLink { get; set; }

    [BsonElement("other_Contact_Link")]
    public string OtherContactLink { get; set; }

    [BsonElement("profile_Photo_Path")]
    public string ProfilePhotoPath { get; set; }
}