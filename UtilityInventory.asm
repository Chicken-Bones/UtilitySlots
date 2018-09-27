using Terraria
using CodeChicken.UtilitySlots

METHOD Main::DrawInventory {
	SEG { #if (Main.EquipPage == 1)
		Br end_if
		POINT else_if
		Ldsfld Main::EquipPage
		Ldc_I4 1
	}
	INSERT { #if (Main.EquipPage == UtilityInventory.EquipPage) UtilityInventory.DrawInventory(Main.mH) else
		Ldsfld Main::EquipPage
		Ldc_I4 EquipPage
		Bne_Un cont
		Ldsfld Main::mH
		HOOK UtilityInventory::DrawInventory
		Br end_if
		LABEL cont
	} AT else_if
}
METHOD Main::DrawPageIcons {
	SEG {
		Call Vector2::.ctor(float, float)
	} new_vec
	INSERT { #if(UtilityInventory.DrawPageIcons(vector)) num = UtilityInventory.EquipPage;
		Ldloc l1 #vector
		HOOK UtilityInventory::DrawPageIcons
		Brfalse skip
		Ldc_I4 EquipPage
		Stloc l0 #num
		LABEL skip
	} AFTER FIRST new_vec
	REPLACE ALL { #vector.X += 82 -> 52
		Ldc_R4 82
	} WITH {
		Ldc_R4 52
	}
	REPLACE { #vector.X -= 48 -> 40
		Ldc_R4 48
	} WITH {
		Ldc_R4 40
	}
	INSERT {
		HOOK UtilityInventory::ItemEquipPage
	} BEFORE { #return num
		*
		Ret
	}
}
METHOD ItemSlot::SwapEquip {
	SEG { #if (Main.projHook[inv[slot].shoot])
		Br end_if
		POINT else_if
		Ldsfld Main::projHook
	}
	INSERT { #if(UtilityInventory.ArmorSwapHook(player, inv, slot)) {} else
		Ldloc l0
		Ldarg p0
		Ldarg p2
		HOOK UtilityInventory::ArmorSwapHook
		Brtrue end_if
	} AT else_if