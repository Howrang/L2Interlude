﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataBase.Entities;
using DataBase.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Module.CharacterData
{
    public class CharacterList
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IUserItemRepository _userItemRepository;
        private readonly IDictionary<int, int> _userItem;
        private readonly List<CharacterListModel> _listModels;
        public CharacterList()
        {
            _characterRepository = Initializer.ServiceProvider.GetRequiredService<IUnitOfWorkGame>().Characters;
            _userItemRepository = Initializer.ServiceProvider.GetRequiredService<IUnitOfWorkGame>().UserItems;
            _userItem = new Dictionary<int, int>();
            _listModels = new List<CharacterListModel>();
        }

        public async Task<List<CharacterListModel>> GetCharacterList(string accountName)
        {
            var list = await _characterRepository.GetCharactersByAccountNameAsync(accountName);

            // Process all characters in parallel
            var tasks = list.Select(PrepareCharacterListAsync);
            var characterListModels = await Task.WhenAll(tasks);

            return characterListModels.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private async Task<CharacterListModel> PrepareCharacterListAsync(CharacterEntity entity)
        {
            var items = await _userItemRepository.GetInventoryItemsByOwnerId(entity.CharacterId);
            items.ForEach(item => { _userItem.Add(item.UserItemId, item.ItemId); });

            return new CharacterListModel
            {
                CharacterId = entity.CharacterId,
                CharacterName = entity.CharacterName,
                Level = entity.Level,
                ClassId = entity.ClassId,
                Race = entity.Race,
                Gender = entity.Gender,
                X = entity.XLoc,
                Y = entity.YLoc,
                Z = entity.ZLoc,
                Hp = entity.Hp,
                Mp = entity.Mp,
                MaxHp = entity.MaxHp,
                MaxMp = entity.MaxMp,
                Sp = entity.Sp,
                Exp = entity.Exp,
                Pk = entity.Pk,
                HairShapeIndex = entity.HairShapeIndex,
                HairColorIndex = entity.HairColorIndex,
                FaceIndex = entity.FaceIndex,
                StUnderwear = entity.StUnderwear,
                StRightEar = entity.StRightEar,
                StLeftEar = entity.StLeftEar,
                StNeck = entity.StNeck,
                StRightFinger = entity.StRightFinger,
                StLeftFinger = entity.StLeftFinger,
                StHead = entity.StHead,
                StRightHand = entity.StRightHand,
                StLeftHand = entity.StLeftHand,
                StGloves = entity.StGloves,
                StChest = entity.StChest,
                StLegs = entity.StLegs,
                StFeet = entity.StFeet,
                StBack = entity.StBack,
                StBothHand = entity.StBothHand,
                StHair = entity.StHair,
                StFace = entity.StFace,
                StHairAll = entity.StHairAll,
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userItemId"></param>
        /// <returns></returns>
        public int GetItem(int userItemId)
        {
            return _userItem.TryGetValue(userItemId, out var itemId) ? itemId : 0;
        }
    }
}