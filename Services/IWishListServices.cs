﻿using Nwazet.Commerce.Models;
using Orchard;
using Orchard.Security;
using System.Collections.Generic;

namespace Nwazet.Commerce.Services {
    public interface IWishListServices : IDependency {
        /// <summary>
        /// Get the wishlists owned by the specified user.
        /// </summary>
        /// <param name="user">The user whose wishlists we are trying to get.</param>
        /// <param name="max">Optionally, tell how many wishlists this call can return.</param>
        /// <returns>An IEnumerable<WishListListPart> of the requested wishlists.</returns>
        IEnumerable<WishListListPart> GetWishLists(IUser user, int max = 0);
        /// <summary>
        /// Gets the default wishlist for the specified user.
        /// If the user does not have a default wishlist yet, one is created.
        /// </summary>
        /// <param name="user">The user whose wishlists we are trying to get.</param>
        /// <returns>The user's default wishlist.</returns>
        WishListListPart GetDefaultWishList(IUser user);
        /// <summary>
        /// Gets a specific wishlist of the specified user.
        /// If the specific wishlist cannot be found or accessed, the default wishlist is 
        /// returned instead.
        /// </summary>
        /// <param name="user">The user whose wishlists we are trying to get.</param>
        /// <param name="wishListId">The id of the wishlist.</param>
        /// <returns>The specific wishlist.</returns>
        WishListListPart GetWishList(IUser user, int wishListId = 0);
        /// <summary>
        /// Gets a specific wishlist of the specified user.
        /// If the specific wishlist cannot be found or accessed, the default wishlist is 
        /// returned instead.
        /// </summary>
        /// <param name="user">The user whose wishlists we are trying to get.</param>
        /// <param name="wishList">The wish listwe found</param>
        /// <param name="wishListId">The id of the wishlist.</param>
        /// <returns>True if the specific wishlist was found. False otherwise.
        /// In the latter case, the WishListPart in out is the default one.</returns>
        bool TryGetWishList(IUser user, out WishListListPart wishList, int wishListId = 0);
        /// <summary>
        /// Gets a specific wishlist, no matter who owns it.
        /// If the specific wishlist cannot be found or accessed, the default wishlist is 
        /// returned instead.
        /// </summary>
        /// <param name="wishList">The wish listwe found</param>
        /// <param name="wishListId">The id of the wishlist.</param>
        /// <returns>True if the specific wishlist was found. False otherwise.
        /// In the latter case, the WishListPart in out is the default one.</returns>
        bool TryGetWishList(out WishListListPart wishList, int wishListId = 0);
        /// <summary>
        /// Creates a new wish list for the given user and with the given title
        /// </summary>
        /// <param name="user">The user for whom a wish list is being created.</param>
        /// <param name="title">The title of the new wish list</param>
        /// <returns>The newly created wish list</returns>
        WishListListPart CreateWishList(IUser user, string title = null);
        /// <summary>
        /// Adds the specific product, with its attributes, to the wish list.
        /// </summary>
        /// <param name="user">The user whose wishlist we are updating</param>
        /// <param name="wishlist">The wish list we are updating</param>
        /// <param name="product">The product to add</param>
        /// <param name="attributes">The product's attributes</param>
        void AddProductToWishList(IUser user, WishListListPart wishlist, ProductPart product, IDictionary<int, ProductAttributeValueExtended> attributes);
        /// <summary>
        /// Remove the specific element from the wishilst given. This does an hard delete on the element.
        /// </summary>
        /// <param name="wishlist">The wish list we are updating</param>
        /// <param name="itemId">The id of the element to remove</param>
        void RemoveItemFromWishlist(WishListListPart wishlist, int itemId);
        /// <summary>
        /// Deletes the wishlist and all its elements. This is an hard delete.
        /// </summary>
        /// <param name="wishlist">The wish list we are deleting</param>
        void DeleteWishlist(WishListListPart wishlist);
       
    }
}
