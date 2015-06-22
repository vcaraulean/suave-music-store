module SuaveMusicStore.Db

open FSharp.Data.Sql

type Sql = SqlDataProvider<"Server=.\\SQLEXPRESS;Database=SuaveMusicStore;Trusted_Connection=True;MultipleActiveResultSets=true", 
                            DatabaseVendor=Common.DatabaseProviderTypes.MSSQLSERVER >

type DbContext = Sql.dataContext
type Album = DbContext.``[dbo].[Albums]Entity``
type Genre = DbContext.``[dbo].[Genres]Entity``
type AlbumDetails = DbContext.``[dbo].[AlbumDetails]Entity``

let getContext() = Sql.GetDataContext()

let firstOrNone s = s |> Seq.tryFind (fun _ -> true)

let getGenres (ctx : DbContext) : Genre list =
    ctx.``[dbo].[Genres]`` |> Seq.toList

let getAlbumsForGenre genreName (ctx : DbContext) : Album list =
    query{
        for album in ctx.``[dbo].[Albums]`` do
            join genre in ctx.``[dbo].[Genres]`` on (album.GenreId = genre.GenreId)
            where (genre.Name = genreName)
            select album
    }
    |> Seq.toList

let getAlbumDetails id (ctx : DbContext) : AlbumDetails option = 
    query {
        for album in ctx.``[dbo].[AlbumDetails]`` do
            where (album.AlbumId = id)
            select album
    }
    |> firstOrNone


let getAlbumsDetails (ctx : DbContext) : AlbumDetails list =
    ctx.``[dbo].[AlbumDetails]`` |> Seq.sortBy (fun f -> f.Artist) |> Seq.toList


let getAlbum id (ctx:DbContext) : Album option = 
    query {
        for album in ctx.``[dbo].[Albums]`` do
        where (album.AlbumId = id)
        select album
    }
    |> firstOrNone

let deleteAlbum (album : Album) (ctx:DbContext) =
    album.Delete()
    ctx.SubmitUpdates()

let getArtists (ctx : DbContext) = 
    ctx.``[dbo].[Artists]`` |> Seq.toList

let createAlbum (artistId : int, genreId : int, title : string, price : decimal) (ctx : DbContext) =
    ctx.``[dbo].[Albums]``.Create(artistId, genreId, price, title) |> ignore
    ctx.SubmitUpdates()

let updateAlbum (album : Album) (artistId : int, genreId : int, title : string, price : decimal) (ctx : DbContext) =
    album.Title <- title
    album.Price <- price
    album.ArtistId <- artistId
    album.GenreId <- genreId
    ctx.SubmitUpdates()
    
