using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

[BsonIgnoreExtraElements]
public class dormListing
{
    [BsonId]
    public int Id { get; set; }

    [BsonElement("owner_Id")]
    public int OwnerId { get; set; }

    [BsonElement("dormitory_Name")]
    public string DormitoryName { get; set; }

    [BsonElement("address_Location")]
    public string AddressLocation { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("near_Schools")]
    public List<string> NearSchools { get; set; } = new List<string>();

    [BsonElement("photo_Paths")]
    public List<string> PhotoPaths { get; set; } = new List<string>();

    [BsonElement("amenities")]
    public List<string> Amenities { get; set; } = new List<string>();

    [BsonElement("curfew")]
    public string Curfew { get; set; }

    [BsonElement("comfort_Room")]
    public string ComfortRoom { get; set; }

    [BsonElement("room_Type")]
    public string RoomType { get; set; }

    [BsonElement("monthly_Rent")]
    public string MonthlyRent { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } // "Draft", "Vacant", or "Occupied"

    [BsonElement("rooms")]
    public List<roomTypeEntry> Rooms { get; set; } = new List<roomTypeEntry>();

    [BsonElement("date_Created")]
    public DateTime DateCreated { get; set; }
}

public class roomTypeEntry
{
    [BsonElement("room_Type_Name")]
    public string RoomTypeName { get; set; }

    [BsonElement("price_Per_Month")]
    public string PricePerMonth { get; set; }

    [BsonElement("availability")]
    public string Availability { get; set; } = "Available"; // "Available" or "Full"
}