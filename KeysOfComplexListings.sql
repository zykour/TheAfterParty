﻿select * from dbo.ProductKeys C right join (select ListingID as listA, ListingName from dbo.Listings A right join (select Listing_ListingID as list from dbo.ListingListings group by Listing_ListingID) B on A.ListingID = B.list where Quantity > 0) D on C.ListingID = D.listA
select * from dbo.Listings A right join (select Listing_ListingID as list from dbo.ListingListings group by Listing_ListingID) B on A.ListingID = B.list where Quantity > 0