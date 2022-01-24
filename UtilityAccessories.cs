using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UtilitySlots.Handlers;

namespace UtilitySlots
{
    public class UtilityAccessories : ILoadable
	{
		private static List<Func<Item, UtilityItemHandler>> providers = new List<Func<Item, UtilityItemHandler>>();
		private static Dictionary<int, UtilityItemHandler> itemIdHandlers = new Dictionary<int, UtilityItemHandler>();

		public static UtilityItemHandler GetHandler(Item item) =>
			providers.Select(provider => provider(item)).FirstOrDefault(handler => handler != null);

		public static void AddProvider(Func<Item, UtilityItemHandler> provider) => providers.Add(provider);

		public static void AddHandler(int itemId, UtilityItemHandler handler) => itemIdHandlers[itemId] = handler;

		public static void AddUtilityAccessory(params int[] itemIds) {
			foreach (var itemId in itemIds)
				AddHandler(itemId, new FullyFunctionalHandler(itemId));
		}

		void ILoadable.Load(Mod mod) {
			// default handler provider
			AddProvider(item => {
				itemIdHandlers.TryGetValue(item.type, out var h);
				return h;
			});

			// wing provider
			AddProvider(item => item.wingSlot > 0 ? new FullyFunctionalHandler(item.type) : null);

			// music box provider
			//AddProvider(item => MusicLoader.itemToMusic.ContainsKey(item.type) ? new FullyFunctionalHandler(item.type) : null);
		}

		void ILoadable.Unload() {
			providers.Clear();
			itemIdHandlers.Clear();
		}
    }
}
