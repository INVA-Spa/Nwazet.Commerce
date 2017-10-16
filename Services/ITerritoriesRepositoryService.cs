﻿using Nwazet.Commerce.Models;
using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.Services {
    /// <summary>
    /// This interface defines the CRUD operations to the storage for TerritoryInternalRecord objects.
    /// </summary>
    public interface ITerritoriesRepositoryService : IDependency {

        /// <summary>
        /// Get the TerritoryInternalRecord object with the given id.
        /// </summary>
        /// <param name="id">The id of the desired object.</param>
        /// <returns>A deep copy of the desired TerritoryInternalRecord object</returns>
        TerritoryInternalRecord GetTerritoryInternal(int id);

        /// <summary>
        /// Get the TerritoryInternalRecord object with the given name.
        /// </summary>
        /// <param name="name">The name of the desired object.</param>
        /// <returns>A deep copy of the desired TerritoryInternalRecord object</returns>
        TerritoryInternalRecord GetTerritoryInternal(string name);

        /// <summary>
        /// Gets the TerritoryInternalRecord objects based on the pagination
        /// </summary>
        /// <param name="startIndex">Start index for pagination</param>
        /// <param name="pageSize">Page size (maximum number of objects)</param>
        /// <returns>An IEnumerable of TerritoryInternalRecord objects, that are deep copies 
        /// of the objects in the storage.</returns>
        IEnumerable<TerritoryInternalRecord> GetTerritories(int startIndex = 0, int pageSize = 0);

        /// <summary>
        /// Get the TerritoryInternalRecord objects with the give ids.
        /// </summary>
        /// <param name="itemIds">The ids of the TerritoryInternalRecord objects</param>
        /// <returns>An IEnumerable of TerritoryInternalRecord objects, that are deep copies 
        /// of the objects in the storage.</returns>
        IEnumerable<TerritoryInternalRecord> GetTerritories(int[] itemIds);

        /// <summary>
        /// Get the total number of TerritoryInternalRecord objects in the storage.
        /// </summary>
        /// <returns>The total number of objects.</returns>
        int GetTerritoriesCount();

        /// <summary>
        /// Adds a new TerritoryInternalRecord object to the repository.
        /// </summary>
        /// <param name="tir">The TerritoryInternalRecord object we wish to add</param>
        /// <returns>A deep copy of the new TerritoryInternalRecord object</returns>
        /// <exception cref="TerritoryInternalDuplicateException">Throws a TerritoryInternalDuplicateException 
        /// if a TerritoryInternalRecord with the same Name already exists.</exception>  
        TerritoryInternalRecord AddTerritory(TerritoryInternalRecord tir);

        /// <summary>
        /// Updates the TerritoryInternalRecord object. The object is identified by the id.
        /// </summary>
        /// <param name="tir">The TerritoryInternalRecord object we wish to update.</param>
        /// <returns>A deep copy of the updated TerritoryInternalRecord object</returns>
        /// <exception cref="TerritoryInternalDuplicateException">Throws a TerritoryInternalDuplicateException 
        /// if a TerritoryInternalRecord with the same Name already exists.</exception>  
        TerritoryInternalRecord Update(TerritoryInternalRecord tir);

        /// <summary>
        /// Deletes the TerritoryInternalRecord object given by the id.
        /// </summary>
        /// <param name="id">The id of the TerritoryInternalRecord object to be deleted</param>
        void Delete(int id);

        /// <summary>
        /// Deletes the TerritoryInternalRecord object. The object is identified by the id.
        /// </summary>
        /// <param name="tir">The TerritoryInternalRecord object to be deleted.</param>
        void Delete(TerritoryInternalRecord tir);
    }
}
