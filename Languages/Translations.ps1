<# This is only marginally less painful than having them split across fifty-four bleeding files. #>

$Translations = [Ordered] @{
	English = [Ordered] @{
		'Keyed/RimFridge' = [Ordered] @{
			'RimFridge.TargetTemperature' = 'Target Temperature'
			'RimFridge.CurrentTemperature' = 'Current Temperature'
			'RimFridge.Power' = 'Power'
			'RimFridge.RenameTheRefrigerator' = 'Rename the refrigerator'
			'RimFridge.ToggleGlowColor' = 'Toggle Glow Color'
			'RimFridge.ToggleGlowColorDesc' = 'Toggle the color of the glow between normal and dark light.'
			'RimFridge.Compatibility' = 'Compatibility'
			'RimFridge.ForceApplicationOfThesePatches' = 'Force the application of these patches for other versions of RimFridge.'
			'RimFridge.ModifyBasePowerRequirement' = 'Modify Base Power Requirement'
			'RimFridge.Apply' = 'Apply'
			'RimFridge.PowerFactorExplanation' = 'New Power Usage = Input Value * Original Power Usage'
			'RimFridge.BasePowerFactor' = 'Base Power Factor'
			'RimFridge.CannotBeLessThanOrEqualToZero' = '{0} cannot be less than or equal to 0.'
			'RimFridge.UnableToParseToANumber' = 'Unable to parse {0} to a number.'
			'RimFridge.NewPowerFactorApplied' = 'New Power Factor Applied'
			'RimFridge.ActAsTradeBeacon' = 'Act as Trade Beacon'
			'RimFridge.FrostyBeverage' = 'Frosty Beverage'
			'RimFridge.EnableFrostyBeverages' = 'Enable Frosty Beverages'
		} <# END Keyed/RimFridge #>
		'DefInjected/ThingDef/Fridge_Building' = [Ordered] @{
			'RimFridge_SingleRefrigerator.building.groupingLabel' = 'Single Refrigerator'
			'RimFridge_Refrigerator.building.groupingLabel' = 'Dual Refrigerator'
			'RimFridge_QuadRefrigerator.building.groupingLabel' = 'Quad Refrigerator'
		} <# END DefInjected/ThingDef/Fridge_Building #>
		'DefInjected/ThingDef/WallFridge_Building' = [Ordered] @{
			'RimFridge_SingleWallRefrigerator.building.groupingLabel' = 'Wall Single Refrigerator'
			'RimFridge_WallRefrigerator.building.groupingLabel' = 'Wall Dual Refrigerator'
		} <# END DefInjected/ThingDef/WallFridge_Building #>
	}

	ChineseSimplified = [Ordered] @{
		'Keyed/RimFridge' = [Ordered] @{
			'RimFridge.TargetTemperature' = '目标温度'
			'RimFridge.CurrentTemperature' = '当前温度'
			'RimFridge.Power' = '功率'
			'RimFridge.RenameTheRefrigerator' = '重命名此冰箱'
			'RimFridge.ToggleGlowColor' = '切换发光颜色'
			'RimFridge.ToggleGlowColorDesc' = '在正常光和暗光之间切换发光颜色。'
			'RimFridge.Compatibility' = '兼容性'
			'RimFridge.ForceApplicationOfThesePatches' = '强制为其他版本的 RimFridge 应用这些补丁。'
			'RimFridge.ModifyBasePowerRequirement' = '修改基本功率要求'
			'RimFridge.Apply' = '应用'
			'RimFridge.PowerFactorExplanation' = '新电力消耗 = 输入值 * 原电力消耗'
			'RimFridge.BasePowerFactor' = '基本功率因数'
			'RimFridge.CannotBeLessThanOrEqualToZero' = '{0} 不能小于或等于0。'
			'RimFridge.UnableToParseToANumber' = '无法将 {0} 解析为一个数字。'
			'RimFridge.NewPowerFactorApplied' = '新的功率因数已应用'
			'RimFridge.ActAsTradeBeacon' = '充当贸易标志'
			'RimFridge.FrostyBeverage' = '冰爽饮料'
			'RimFridge.EnableFrostyBeverages' = '启用冰爽饮料'
		} <# END Keyed/RimFridge #>
		'DefInjected/ThingDef/Fridge_Building' = [Ordered] @{
			'RimFridge_Refrigerator.label' = '冰箱'
			'RimFridge_Refrigerator.description' = '食物放在里面不会腐坏。'
			'RimFridge_Refrigerator_Blueprint.label' = '冰箱（蓝图）'
			'RimFridge_Refrigerator_Blueprint_Install.label' = '冰箱（蓝图）'
			'RimFridge_Refrigerator_Frame.label' = '冰箱（建造中）'
			'RimFridge_Refrigerator_Frame.description' = '食物放在里面不会腐坏。'
			'RimFridge_SingleRefrigerator.label' = '小冰箱'
			'RimFridge_SingleRefrigerator.description' = '食物放在里面不会腐坏。'
			'RimFridge_SingleRefrigerator_Blueprint.label' = '小冰箱（蓝图）'
			'RimFridge_SingleRefrigerator_Blueprint_Install.label' = '小冰箱（蓝图）'
			'RimFridge_SingleRefrigerator_Frame.label' = '小冰箱（建造中）'
			'RimFridge_SingleRefrigerator_Frame.description' = '食物放在里面不会腐坏。'
			'RimFridge_QuadRefrigerator.label' = '大冰箱'
			'RimFridge_QuadRefrigerator.description' = '食物放在里面不会腐坏。'
			'RimFridge_QuadRefrigerator_Blueprint.label' = '大冰箱（蓝图）'
			'RimFridge_QuadRefrigerator_Blueprint_Install.label' = '大冰箱（蓝图）'
			'RimFridge_QuadRefrigerator_Frame.label' = '大冰箱（建造中）'
			'RimFridge_QuadRefrigerator_Frame.description' = '食物放在里面不会腐坏。'
			'RimFridge_SingleRefrigerator.building.groupingLabel' = '小冰箱'
			'RimFridge_Refrigerator.building.groupingLabel' = '冰箱'
			'RimFridge_QuadRefrigerator.building.groupingLabel' = '大冰箱'
		} <# END DefInjected/ThingDef/Fridge_Building #>
		'DefInjected/ThingDef/WallFridge_Building' = [Ordered] @{
			'RimFridge_WallRefrigerator.label' = '壁挂式冰箱'
			'RimFridge_WallRefrigerator.description' = '食物放在里面不会腐坏。'
			'RimFridge_WallRefrigerator_Blueprint.label' = '壁挂式冰箱（蓝图）'
			'RimFridge_WallRefrigerator_Blueprint_Install.label' = '壁挂式冰箱（蓝图）'
			'RimFridge_WallRefrigerator_Frame.label' = '壁挂式冰箱（建造中）'
			'RimFridge_WallRefrigerator_Frame.description' = '食物放在里面不会腐坏。'
			'RimFridge_SingleWallRefrigerator.label' = '小型壁挂式冰箱'
			'RimFridge_SingleWallRefrigerator.description' = '食物放在里面不会腐坏。'
			'RimFridge_SingleWallRefrigerator_Blueprint.label' = '小型壁挂式冰箱（蓝图）'
			'RimFridge_SingleWallRefrigerator_Blueprint_Install.label' = '小型壁挂式冰箱（蓝图）'
			'RimFridge_SingleWallRefrigerator_Frame.label' = '小型壁挂式冰箱（建造中）'
			'RimFridge_SingleWallRefrigerator_Frame.description' = '食物放在里面不会腐坏。'
			'RimFridge_SingleWallRefrigerator.building.groupingLabel' = '小型壁挂式冰箱'
			'RimFridge_WallRefrigerator.building.groupingLabel' = '壁挂式冰箱'
		} <# END DefInjected/ThingDef/WallFridge_Building #>
		'DefInjected/ThoughtDef/FrostyBeer' = [Ordered] @{
			'FrostyBeer.stages.0.label' = '享受冰镇饮品'
			'FrostyBeer.stages.0.description' = '没什么能比冰镇饮料更棒了！'
		} <# END DefInjected/ThoughtDef/FrostyBeer #>
	}

	ChineseTraditional = [Ordered] @{
		'Keyed/RimFridge' = [Ordered] @{
			'RimFridge.TargetTemperature' = '目标温度'
			'RimFridge.CurrentTemperature' = '当前温度'
			'RimFridge.Power' = '功率'
			'RimFridge.RenameTheRefrigerator' = '重命名此冰箱'
			'RimFridge.ToggleGlowColor' = '切换发光颜色'
			'RimFridge.ToggleGlowColorDesc' = '在正常光和暗光之间切换发光颜色。'
			'RimFridge.Compatibility' = '兼容性'
			'RimFridge.ForceApplicationOfThesePatches' = '强制为其他版本的 RimFridge 应用这些补丁。'
			'RimFridge.ModifyBasePowerRequirement' = '修改基本功率要求'
			'RimFridge.Apply' = '应用'
			'RimFridge.PowerFactorExplanation' = '新电力消耗 = 输入值 * 原电力消耗'
			'RimFridge.BasePowerFactor' = '基本功率因数'
			'RimFridge.CannotBeLessThanOrEqualToZero' = '{0} 不能小于或等于0。'
			'RimFridge.UnableToParseToANumber' = '无法将 {0} 解析为一个数字。'
			'RimFridge.NewPowerFactorApplied' = '新的功率因数已应用'
			'RimFridge.ActAsTradeBeacon' = '充当贸易标志'
			'RimFridge.FrostyBeverage' = '冰爽飲料'
			'RimFridge.EnableFrostyBeverages' = '啟用冰爽飲料'
		} <# END Keyed/RimFridge #>
		'DefInjected/ThingDef/Fridge_Building' = [Ordered] @{
			'RimFridge_Refrigerator.label' = '冰箱'
			'RimFridge_Refrigerator.description' = '食物放在裡面不會腐壞。'
			'RimFridge_Refrigerator_Blueprint.label' = '冰箱（藍圖）'
			'RimFridge_Refrigerator_Blueprint_Install.label' = '冰箱（藍圖）'
			'RimFridge_Refrigerator_Frame.label' = '冰箱（建造中）'
			'RimFridge_Refrigerator_Frame.description' = '食物放在裡面不會腐壞。 '
			'RimFridge_SingleRefrigerator.label' = '小型冰箱'
			'RimFridge_SingleRefrigerator.description' = '食物放在裡面不會腐壞。'
			'RimFridge_SingleRefrigerator_Blueprint.label' = '小型冰箱（藍圖）'
			'RimFridge_SingleRefrigerator_Blueprint_Install.label' = '小型冰箱（藍圖）'
			'RimFridge_SingleRefrigerator_Frame.label' = '小型冰箱（建造中）'
			'RimFridge_SingleRefrigerator_Frame.description' = '食物放在裡面不會腐壞。'
			'RimFridge_QuadRefrigerator.label' = '大型冰箱'
			'RimFridge_QuadRefrigerator.description' = '食物放在裡面不會腐壞。'
			'RimFridge_QuadRefrigerator_Blueprint.label' = '大型冰箱（藍圖）'
			'RimFridge_QuadRefrigerator_Blueprint_Install.label' = '大型冰箱（藍圖）'
			'RimFridge_QuadRefrigerator_Frame.label' = '大型冰箱（建造中）'
			'RimFridge_QuadRefrigerator_Frame.description' = '食物放在裡面不會腐壞。'
			'RimFridge_SingleRefrigerator.building.groupingLabel' = '小型冰箱'
			'RimFridge_Refrigerator.building.groupingLabel' = '冰箱'
			'RimFridge_QuadRefrigerator.building.groupingLabel' = '大型冰箱'
		} <# END DefInjected/ThingDef/Fridge_Building #>
		'DefInjected/ThingDef/WallFridge_Building' = [Ordered] @{
			'RimFridge_WallRefrigerator.label' = '壁掛式冰箱'
			'RimFridge_WallRefrigerator.description' = '食物放在裡面不會腐壞。'
			'RimFridge_WallRefrigerator_Blueprint.label' = '壁掛式冰箱（藍圖）'
			'RimFridge_WallRefrigerator_Blueprint_Install.label' = '壁掛式冰箱（藍圖）'
			'RimFridge_WallRefrigerator_Frame.label' = '壁掛式冰箱（建造中）'
			'RimFridge_WallRefrigerator_Frame.description' = '食物放在裡面不會腐壞。'
			'RimFridge_SingleWallRefrigerator.label' = '小型壁掛式冰箱'
			'RimFridge_SingleWallRefrigerator.description' = '食物放在裡面不會腐壞。'
			'RimFridge_SingleWallRefrigerator_Blueprint.label' = '小型壁掛式冰箱（藍圖）'
			'RimFridge_SingleWallRefrigerator_Blueprint_Install.label' = '小型壁掛式冰箱（藍圖）'
			'RimFridge_SingleWallRefrigerator_Frame.label' = '小型壁掛式冰箱（建造中）'
			'RimFridge_SingleWallRefrigerator_Frame.description' = '食物放在裡面不會腐壞。'
			'RimFridge_SingleWallRefrigerator.building.groupingLabel' = '小型壁掛式冰箱'
			'RimFridge_WallRefrigerator.building.groupingLabel' = '壁掛式冰箱'
		} <# END DefInjected/ThingDef/WallFridge_Building #>
		'DefInjected/ThoughtDef/FrostyBeer' = [Ordered] @{
			'FrostyBeer.stages.0.label' = '享受冰鎮飲品'
			'FrostyBeer.stages.0.description' = '沒什麼能比冰鎮飲料更棒了！'
		} <# END DefInjected/ThoughtDef/FrostyBeer #>
	}

	French = [Ordered] @{
		'Keyed/RimFridge' = [Ordered] @{
			'RimFridge.TargetTemperature' = 'Température Cible'
			'RimFridge.CurrentTemperature' = 'Température Actuelle'
			'RimFridge.Power' = 'Énergie'
			'RimFridge.RenameTheRefrigerator' = 'Renommer le Réfrigérateur'
			'RimFridge.ToggleGlowColor' = 'Changer la Couleur de la Lueur'
			'RimFridge.ToggleGlowColorDesc' = 'Changer la couleur de la lueur entre lumière normale et lumière sombre.'
			'RimFridge.Compatibility' = 'Compatibilité'
			'RimFridge.ForceApplicationOfThesePatches' = 'Forcer l''application de ces correctifs pour d''autres versions de RimFridge.'
			'RimFridge.ModifyBasePowerRequirement' = 'Modifier la Consommation d''Énergie de Base'
			'RimFridge.Apply' = 'Appliquer'
			'RimFridge.PowerFactorExplanation' = 'Nouvelle consommation d''énergie = Valeur saisie * Consommation d''origine'
			'RimFridge.BasePowerFactor' = 'Facteur de Consommation de Base'
			'RimFridge.CannotBeLessThanOrEqualToZero' = '{0} ne peut pas être inférieur ou égal à 0.'
			'RimFridge.UnableToParseToANumber' = 'Impossible d''analyser {0} en un nombre.'
			'RimFridge.NewPowerFactorApplied' = 'Nouveau Facteur de Consommation Appliqué'
			'RimFridge.ActAsTradeBeacon' = 'Agir comme Balise Commerciale'
			'RimFridge.FrostyBeverage' = 'Boissons Glacées'
			'RimFridge.EnableFrostyBeverages' = 'Activer les Boissons Glacées'
		} <# END Keyed/RimFridge #>
		'DefInjected/ThingDef/Fridge_Building' = [Ordered] @{
			'RimFridge_Refrigerator.label' = 'Réfrigérateur'
			'RimFridge_Refrigerator.description' = 'Les périssables stockés à l''intérieur ne pourriront pas.'
			'RimFridge_SingleRefrigerator.label' = 'Petit réfrigérateur'
			'RimFridge_SingleRefrigerator.description' = 'Les périssables stockés à l''intérieur ne pourriront pas.'
			'RimFridge_SingleRefrigerator.building.groupingLabel' = 'Petit réfrigérateur'
			'RimFridge_Refrigerator.building.groupingLabel' = 'Réfrigérateur'
			'RimFridge_QuadRefrigerator.building.groupingLabel' = ''
		} <# END DefInjected/ThingDef/Fridge_Building #>
		'DefInjected/ThingDef/WallFridge_Building' = [Ordered] @{
			'RimFridge_WallRefrigerator.label' = 'Réfrigérateur mural double'
			'RimFridge_WallRefrigerator.description' = 'Les denrées périssables stockées ici ne se détérioreront pas.'
			'RimFridge_SingleWallRefrigerator.label' = 'Réfrigérateur mural simple'
			'RimFridge_SingleWallRefrigerator.description' = 'Les denrées périssables stockées ici ne se détérioreront pas.'
			'RimFridge_SingleWallRefrigerator.building.groupingLabel' = 'Réfrigérateur mural simple'
			'RimFridge_WallRefrigerator.building.groupingLabel' = 'Réfrigérateur mural double'
		} <# END DefInjected/ThingDef/WallFridge_Building #>
		'DefInjected/ThoughtDef/FrostyBeer' = [Ordered] @{
			'FrostyBeer.stages.0.label' = 'Appréciez-en une bien fraîche'
			'FrostyBeer.stages.0.description' = 'Rien de mieux qu''une boisson fraîche !'
		} <# END DefInjected/ThoughtDef/FrostyBeer #>
	}

	German = [Ordered] @{
		'Keyed/RimFridge' = [Ordered] @{
			'RimFridge.TargetTemperature' = 'Zieltemperatur'
			'RimFridge.CurrentTemperature' = 'Aktuelle Temperatur'
			'RimFridge.Power' = 'Strom'
			'RimFridge.RenameTheRefrigerator' = 'Kühlschrank Umbenennen'
			'RimFridge.ToggleGlowColor' = 'Leuchtfarbe Umschalten'
			'RimFridge.ToggleGlowColorDesc' = 'Wechselt die Farbe der Leuchtkraft zwischen normalem und dunklem Licht.'
			'RimFridge.Compatibility' = 'Kompatibilität'
			'RimFridge.ForceApplicationOfThesePatches' = 'Erzwinge die Anwendung dieser Patches für andere Versionen von RimFridge.'
			'RimFridge.ModifyBasePowerRequirement' = 'Grundenergiebedarf Ändern'
			'RimFridge.Apply' = 'Anwenden'
			'RimFridge.PowerFactorExplanation' = 'Neuer Energieverbrauch = Eingabewert * Ursprünglicher Energieverbrauch'
			'RimFridge.BasePowerFactor' = 'Grundenergie-Faktor'
			'RimFridge.CannotBeLessThanOrEqualToZero' = '{0} darf nicht kleiner oder gleich 0 sein.'
			'RimFridge.UnableToParseToANumber' = '{0} kann nicht in eine Zahl umgewandelt werden.'
			'RimFridge.NewPowerFactorApplied' = 'Neuer Energie-Faktor Angewendet'
			'RimFridge.ActAsTradeBeacon' = 'Als Handelsbake Agieren'
			'RimFridge.FrostyBeverage' = 'Eisgekühlte Getränke'
			'RimFridge.EnableFrostyBeverages' = 'Eisgekühlte Getränke aktivieren'
		} <# END Keyed/RimFridge #>
		'DefInjected/ThingDef/Fridge_Building' = [Ordered] @{
			'RimFridge_Refrigerator.label' = 'Kühlschrank'
			'RimFridge_Refrigerator.description' = 'Verderbliche Waren werden in diesem Kühlschrank nicht verfaulen.'
			'RimFridge_Refrigerator_Blueprint.label' = 'Kühlschrank (Blaupause)'
			'RimFridge_Refrigerator_Blueprint_Install.label' = 'Kühlschrank (Blaupause)'
			'RimFridge_Refrigerator_Frame.label' = 'KühlschrankWandkühlschrank (im Bau)'
			'RimFridge_Refrigerator_Frame.description' = 'Verderbliche Waren werden in diesem Kühlschrank nicht verfaulen.'
			'RimFridge_SingleRefrigerator.label' = 'Mini-Kühlschrank'
			'RimFridge_SingleRefrigerator.description' = 'Verderbliche Waren werden in diesem kleinen Kühlschrank nicht verfaulen.'
			'RimFridge_SingleRefrigerator_Blueprint.label' = 'Mini-Kühlschrank (Blaupause)'
			'RimFridge_SingleRefrigerator_Blueprint_Install.label' = 'Mini-Kühlschrank (Blaupause)'
			'RimFridge_SingleRefrigerator_Frame.label' = 'Mini-KühlschrankWandkühlschrank (im Bau)'
			'RimFridge_SingleRefrigerator_Frame.description' = 'Verderbliche Waren werden in diesem kleinen Kühlschrank nicht verfaulen.'
			'RimFridge_QuadRefrigerator.label' = 'Großer Kühlschrank'
			'RimFridge_QuadRefrigerator.description' = 'Verderbliche Waren werden in diesem großen Kühlschrank nicht verfaulen.'
			'RimFridge_QuadRefrigerator_Blueprint.label' = 'Großer Kühlschrank (Blaupause)'
			'RimFridge_QuadRefrigerator_Blueprint_Install.label' = 'Großer Kühlschrank (Blaupause)'
			'RimFridge_QuadRefrigerator_Frame.label' = 'Großer KühlschrankWandkühlschrank (im Bau)'
			'RimFridge_QuadRefrigerator_Frame.description' = 'Verderbliche Waren werden in diesem großen Kühlschrank nicht verfaulen.'
			'RimFridge_SingleRefrigerator.building.groupingLabel' = 'Mini-Kühlschrank'
			'RimFridge_Refrigerator.building.groupingLabel' = 'Kühlschrank'
			'RimFridge_QuadRefrigerator.building.groupingLabel' = 'Großer Kühlschrank'
		} <# END DefInjected/ThingDef/Fridge_Building #>
		'DefInjected/ThingDef/WallFridge_Building' = [Ordered] @{
			'RimFridge_WallRefrigerator.label' = 'Wandkühlschrank'
			'RimFridge_WallRefrigerator.description' = 'Verderbliche Waren werden in diesem Wandkühlschrank nicht verfaulen. Dieser kann wie eine Wand nicht passiert werden.'
			'RimFridge_WallRefrigerator_Blueprint.label' = 'Wandkühlschrank (Blaupause)'
			'RimFridge_WallRefrigerator_Blueprint_Install.label' = 'Wandkühlschrank (Blaupause)'
			'RimFridge_WallRefrigerator_Frame.label' = 'Wandkühlschrank (im Bau)'
			'RimFridge_WallRefrigerator_Frame.description' = 'Verderbliche Waren werden in diesem Kühlschrank nicht verfaulen. Dieser kann wie eine Wand nicht passiert werden.'
			'RimFridge_SingleWallRefrigerator.label' = 'Doppelter Wandkühlschrank'
			'RimFridge_SingleWallRefrigerator.description' = 'Verderbliche Waren werden in diesem doppelten Wandkühlschrank nicht verfaulen. Dieser kann wie eine Wand nicht passiert werden.'
			'RimFridge_SingleWallRefrigerator_Blueprint.label' = 'Doppelter Wandkühlschrank (Blaupause)'
			'RimFridge_SingleWallRefrigerator_Blueprint_Install.label' = 'Doppelter Wandkühlschrank (Blaupause)'
			'RimFridge_SingleWallRefrigerator_Frame.label' = 'Doppelter Wandkühlschrank (im Bau)'
			'RimFridge_SingleWallRefrigerator_Frame.description' = 'Verderbliche Waren werden in diesem doppelten Wandkühlschrank nicht verfaulen. Dieser kann wie eine Wand nicht passiert werden.'
			'RimFridge_SingleWallRefrigerator.building.groupingLabel' = 'Doppelter Wandkühlschrank'
			'RimFridge_WallRefrigerator.building.groupingLabel' = 'Wandkühlschrank'
		} <# END DefInjected/ThingDef/WallFridge_Building #>
		'DefInjected/ThoughtDef/FrostyBeer' = [Ordered] @{
			'FrostyBeer.stages.0.label' = 'genießte etwas Erfrischendes'
			'FrostyBeer.stages.0.description' = 'Nichts ist besser als ein erfrischendes Getränk!'
		} <# END DefInjected/ThoughtDef/FrostyBeer #>
	}

	Hungarian = [Ordered] @{
		'Keyed/RimFridge' = [Ordered] @{
			'RimFridge.TargetTemperature' = 'Célhőmérséklet'
			'RimFridge.CurrentTemperature' = 'Jelenlegi Hőmérséklet'
			'RimFridge.Power' = 'Energia'
			'RimFridge.RenameTheRefrigerator' = 'Hűtőszekrény Átnevezése'
			'RimFridge.ToggleGlowColor' = 'Fény Színének Váltása'
			'RimFridge.ToggleGlowColorDesc' = 'Váltás a normál és a sötét fény között.'
			'RimFridge.Compatibility' = 'Kompatibilitás'
			'RimFridge.ForceApplicationOfThesePatches' = 'Ezeknek a javításoknak az alkalmazását kényszeríti a RimFridge más verzióira.'
			'RimFridge.ModifyBasePowerRequirement' = 'Alap Energiaigény Módosítása'
			'RimFridge.Apply' = 'Alkalmaz'
			'RimFridge.PowerFactorExplanation' = 'Új energiafogyasztás = Bemeneti érték * Eredeti energiafogyasztás'
			'RimFridge.BasePowerFactor' = 'Alap Energia Faktor'
			'RimFridge.CannotBeLessThanOrEqualToZero' = '{0} nem lehet kisebb vagy egyenlő 0-val.'
			'RimFridge.UnableToParseToANumber' = '{0} nem alakítható számmá.'
			'RimFridge.NewPowerFactorApplied' = 'Új Energia Faktor Alkalmazva'
			'RimFridge.ActAsTradeBeacon' = 'Kereskedelmi Jeladóként Működik'
			'RimFridge.FrostyBeverage' = 'Fagyos Italok'
			'RimFridge.EnableFrostyBeverages' = 'Jéges Italok Engedélyezése'
		} <# END Keyed/RimFridge #>
		'DefInjected/ThingDef/Fridge_Building' = [Ordered] @{
			'RimFridge_Refrigerator.label' = 'Fagyasztó'
			'RimFridge_Refrigerator.description' = 'AZ ételek ebbe a hűtőbe tárolva nem romlanak.'
			'RimFridge_SingleRefrigerator.label' = 'Kis Fagyasztó'
			'RimFridge_SingleRefrigerator.description' = 'AZ ételek ebbe a hűtőbe tárolva nem romlanak.'
			'RimFridge_QuadRefrigerator.label' = 'Nagy Fagasztó'
			'RimFridge_QuadRefrigerator.description' = 'AZ ételek ebbe a hűtőbe tárolva nem romlanak.'
			'RimFridge_SingleRefrigerator.building.groupingLabel' = 'Kis Fagyasztó'
			'RimFridge_Refrigerator.building.groupingLabel' = 'Fagyasztó'
			'RimFridge_QuadRefrigerator.building.groupingLabel' = 'Nagy Fagasztó'
		} <# END DefInjected/ThingDef/Fridge_Building #>
		'DefInjected/ThingDef/WallFridge_Building' = [Ordered] @{
			'RimFridge_SingleWallRefrigerator.building.groupingLabel' = 'Fali Egyetlen Hűtőszekrény'
			'RimFridge_WallRefrigerator.building.groupingLabel' = 'Fali Kettős Hűtőszekrény'
		} <# END DefInjected/ThingDef/WallFridge_Building #>
		'DefInjected/ThoughtDef/FrostyBeer' = [Ordered] @{
			'FrostyBeer.stages.0.label' = 'élvezd a hideget'
			'FrostyBeer.stages.0.description' = 'Semmi sem jobb egy hideg italnál!'
		} <# END DefInjected/ThoughtDef/FrostyBeer #>
	}

	Japanese = [Ordered] @{
		'Keyed/RimFridge' = [Ordered] @{
			'RimFridge.TargetTemperature' = '目標温度'
			'RimFridge.CurrentTemperature' = '現在の温度'
			'RimFridge.Power' = '電力'
			'RimFridge.RenameTheRefrigerator' = '冷蔵庫の名前を変更'
			'RimFridge.ToggleGlowColor' = '発光色の切り替え'
			'RimFridge.ToggleGlowColorDesc' = '発光の色を通常の光と暗い光の間で切り替えます。'
			'RimFridge.Compatibility' = '互換性'
			'RimFridge.ForceApplicationOfThesePatches' = 'RimFridge の他のバージョンにこれらのパッチの適用を強制する'
			'RimFridge.ModifyBasePowerRequirement' = '基本消費電力の変更'
			'RimFridge.Apply' = '適用'
			'RimFridge.PowerFactorExplanation' = '新しい消費電力 = 入力値 × 元の消費電力'
			'RimFridge.BasePowerFactor' = '基本消費電力係数'
			'RimFridge.CannotBeLessThanOrEqualToZero' = '{0} は 0 以下にはできません。'
			'RimFridge.UnableToParseToANumber' = '{0} を数値として解析できません。'
			'RimFridge.NewPowerFactorApplied' = '新しい消費電力係数が適用されました'
			'RimFridge.ActAsTradeBeacon' = '交易ビーコンとして機能'
			'RimFridge.FrostyBeverage' = '冷たい飲み物'
			'RimFridge.EnableFrostyBeverages' = '冷たい飲み物を有効にする'
		} <# END Keyed/RimFridge #>
		'DefInjected/ThingDef/Fridge_Building' = [Ordered] @{
			'RimFridge_SingleRefrigerator.label' = '一人用冷蔵庫'
			'RimFridge_SingleRefrigerator.description' = 'これに保存される鮮度のあるアイテムは、腐敗しません。'
			'Blueprint_RimFridge_SingleRefrigerator.label' = '一人用冷蔵庫(設計)'
			'Blueprint_Install_RimFridge_SingleRefrigerator.label' = '一人用冷蔵庫(移動先)'
			'Frame_RimFridge_SingleRefrigerator.label' = '一人用冷蔵庫(施工)'
			'Frame_RimFridge_SingleRefrigerator.description' = 'これに保存される鮮度のあるアイテムは、腐敗しません。'
			'RimFridge_Refrigerator.label' = '冷蔵庫'
			'RimFridge_Refrigerator.description' = 'これに保存される鮮度のあるアイテムは、腐敗しません。'
			'Blueprint_RimFridge_Refrigerator.label' = '冷蔵庫(設計)'
			'Blueprint_Install_RimFridge_Refrigerator.label' = '冷蔵庫(移動先)'
			'Frame_RimFridge_Refrigerator.label' = '冷蔵庫(施工)'
			'Frame_RimFridge_Refrigerator.description' = 'これに保存される鮮度のあるアイテムは、腐敗しません。'
			'RimFridge_QuadRefrigerator.label' = '冷蔵庫(4連)'
			'RimFridge_QuadRefrigerator.description' = 'これに保存される鮮度のあるアイテムは、腐敗しません。'
			'Blueprint_RimFridge_QuadRefrigerator.label' = '冷蔵庫(4連)(設計)'
			'Blueprint_Install_RimFridge_QuadRefrigerator.label' = '冷蔵庫(4連)(移動先)'
			'Frame_RimFridge_QuadRefrigerator.label' = '冷蔵庫(4連)(施工)'
			'Frame_RimFridge_QuadRefrigerator.description' = 'これに保存される鮮度のあるアイテムは、腐敗しません。'
			'RimFridge_SingleRefrigerator.building.groupingLabel' = '一人用冷蔵庫'
			'RimFridge_Refrigerator.building.groupingLabel' = '冷蔵庫'
			'RimFridge_QuadRefrigerator.building.groupingLabel' = '冷蔵庫(4連)'
		} <# END DefInjected/ThingDef/Fridge_Building #>
		'DefInjected/ThingDef/WallFridge_Building' = [Ordered] @{
			'RimFridge_SingleWallRefrigerator.label' = '壁用冷蔵庫 (一人用)'
			'RimFridge_SingleWallRefrigerator.description' = 'これに保存される鮮度のあるアイテムは、腐敗しません。'
			'Blueprint_RimFridge_SingleWallRefrigerator.label' = '壁用冷蔵庫 (一人用)(設計)'
			'Blueprint_Install_RimFridge_SingleWallRefrigerator.label' = '壁用冷蔵庫 (一人用)(移動先)'
			'Frame_RimFridge_SingleWallRefrigerator.label' = '壁用冷蔵庫 (一人用)(施工)'
			'Frame_RimFridge_SingleWallRefrigerator.description' = 'これに保存される鮮度のあるアイテムは、腐敗しません。'
			'RimFridge_WallRefrigerator.label' = '壁用冷蔵庫 (2連)'
			'RimFridge_WallRefrigerator.description' = 'これに保存される鮮度のあるアイテムは、腐敗しません。'
			'Blueprint_RimFridge_WallRefrigerator.label' = '壁用冷蔵庫 (2連)(設計)'
			'Blueprint_Install_RimFridge_WallRefrigerator.label' = '壁用冷蔵庫 (2連)(移動先)'
			'Frame_RimFridge_WallRefrigerator.label' = '壁用冷蔵庫 (2連)(施工)'
			'Frame_RimFridge_WallRefrigerator.description' = 'これに保存される鮮度のあるアイテムは、腐敗しません。'
			'RimFridge_SingleWallRefrigerator.building.groupingLabel' = '壁用冷蔵庫 (一人用)'
			'RimFridge_WallRefrigerator.building.groupingLabel' = '壁用冷蔵庫 (2連)'
		} <# END DefInjected/ThingDef/WallFridge_Building #>
		'DefInjected/ThoughtDef/FrostyBeer' = [Ordered] @{
			'FrostyBeer.stages.enjoyed_a_cold_one.label' = '冷たい一杯を楽しんだ'
			'FrostyBeer.stages.enjoyed_a_cold_one.description' = '冷やしたビールほどいいものは無い！'
		}
	}

	Korean = [Ordered] @{
		'Keyed/RimFridge' = [Ordered] @{
			'RimFridge.TargetTemperature' = '목표 온도'
			'RimFridge.CurrentTemperature' = '현재 온도'
			'RimFridge.Power' = '전력'
			'RimFridge.RenameTheRefrigerator' = '냉장고 이름 변경'
			'RimFridge.ToggleGlowColor' = '발광 색상 전환'
			'RimFridge.ToggleGlowColorDesc' = '발광 색상을 일반 조명과 어두운 조명 사이에서 전환합니다.'
			'RimFridge.Compatibility' = '호환성'
			'RimFridge.ForceApplicationOfThesePatches' = '다른 RimFridge 버전에 대해 이러한 패치를 강제 적용'
			'RimFridge.ModifyBasePowerRequirement' = '기본 전력 요구량 수정'
			'RimFridge.Apply' = '적용'
			'RimFridge.PowerFactorExplanation' = '새 전력 사용량 = 입력 값 × 기존 전력 사용량'
			'RimFridge.BasePowerFactor' = '기본 전력 계수'
			'RimFridge.CannotBeLessThanOrEqualToZero' = '{0} 은(는) 0 이하일 수 없습니다.'
			'RimFridge.UnableToParseToANumber' = '{0} 을(를) 숫자로 변환할 수 없습니다.'
			'RimFridge.NewPowerFactorApplied' = '새 전력 계수가 적용되었습니다'
			'RimFridge.ActAsTradeBeacon' = '무역 비콘으로 작동'
			'RimFridge.FrostyBeverage' = '시원한 음료'
			'RimFridge.EnableFrostyBeverages' = '시원한 음료 활성화'
		} <# END Keyed/RimFridge #>
		'DefInjected/ThingDef/Fridge_Building' = [Ordered] @{
			'RimFridge_Refrigerator.label' = '냉장고'
			'RimFridge_Refrigerator.description' = '부패할 수 있는 것들이 이 안에 보관되면 썩지 않을 것입니다.'
			'RimFridge_Refrigerator_Blueprint.label' = '냉장고 (청사진)'
			'RimFridge_Refrigerator_Blueprint_Install.label' = '냉장고 (청사진)'
			'RimFridge_Refrigerator_Frame.label' = '냉장고 (건설중)'
			'RimFridge_Refrigerator_Frame.description' = '부패할 수 있는 것들이 이 안에 보관되면 썩지 않을 것입니다.'
			'RimFridge_SingleRefrigerator.label' = '한칸 냉장고'
			'RimFridge_SingleRefrigerator.description' = '부패할 수 있는 것들이 이 안에 보관되면 썩지 않을 것입니다.'
			'RimFridge_SingleRefrigerator_Blueprint.label' = '한칸 냉장고 (청사진)'
			'RimFridge_SingleRefrigerator_Blueprint_Install.label' = '한칸 냉장고 (청사진)'
			'RimFridge_SingleRefrigerator_Frame.label' = '한칸 냉장고 (건설중)'
			'RimFridge_SingleRefrigerator_Frame.description' = '부패할 수 있는 것들이 이 안에 보관되면 썩지 않을 것입니다.'
			'RimFridge_QuadRefrigerator.label' = '네칸 냉장고'
			'RimFridge_QuadRefrigerator.description' = '부패할 수 있는 것들이 이 안에 보관되면 썩지 않을 것입니다.'
			'RimFridge_QuadRefrigerator_Blueprint.label' = '네칸 냉장고 (청사진)'
			'RimFridge_QuadRefrigerator_Blueprint_Install.label' = '네칸 냉장고 (청사진)'
			'RimFridge_QuadRefrigerator_Frame.label' = '네칸 냉장고 (건설중)'
			'RimFridge_QuadRefrigerator_Frame.description' = '부패할 수 있는 것들이 이 안에 보관되면 썩지 않을 것입니다.'
			'RimFridge_SingleRefrigerator.building.groupingLabel' = '한칸 냉장고'
			'RimFridge_Refrigerator.building.groupingLabel' = '냉장고'
			'RimFridge_QuadRefrigerator.building.groupingLabel' = '네칸 냉장고'
		} <# END DefInjected/ThingDef/Fridge_Building #>
		'DefInjected/ThingDef/WallFridge_Building' = [Ordered] @{
			'RimFridge_WallRefrigerator.label' = '벽식 냉장고'
			'RimFridge_WallRefrigerator.description' = '부패할 수 있는 것들이 이 안에 보관되면 썩지 않을 것입니다.'
			'RimFridge_WallRefrigerator_청사진.label' = '벽식 냉장고 (청사진)'
			'RimFridge_WallRefrigerator_청사진_Install.label' = '벽식 냉장고 (청사진)'
			'RimFridge_WallRefrigerator_Frame.label' = '벽식 냉장고 (건설중)'
			'RimFridge_WallRefrigerator_Frame.description' = '부패할 수 있는 것들이 이 안에 보관되면 썩지 않을 것입니다.'
			'RimFridge_SingleWallRefrigerator.label' = '한칸 벽식 냉장고'
			'RimFridge_SingleWallRefrigerator.description' = '부패할 수 있는 것들이 이 안에 보관되면 썩지 않을 것입니다.'
			'RimFridge_SingleWallRefrigerator_청사진.label' = '한칸 벽식 냉장고 (청사진)'
			'RimFridge_SingleWallRefrigerator_청사진_Install.label' = '한칸 벽식 냉장고 (청사진)'
			'RimFridge_SingleWallRefrigerator_Frame.label' = '한칸 벽식 냉장고 (건설중)'
			'RimFridge_SingleWallRefrigerator_Frame.description' = '부패할 수 있는 것들이 이 안에 보관되면 썩지 않을 것입니다.'
			'RimFridge_SingleWallRefrigerator.building.groupingLabel' = '한칸 벽식 냉장고'
			'RimFridge_WallRefrigerator.building.groupingLabel' = '벽식 냉장고'
		} <# END DefInjected/ThingDef/WallFridge_Building #>
		'DefInjected/ThoughtDef/FrostyBeer' = [Ordered] @{
			'FrostyBeer.stages.0.label' = '시원한 음료를 마심'
			'FrostyBeer.stages.0.description' = '차가운 음료만 한 게 없지!'
		} <# END DefInjected/ThoughtDef/FrostyBeer #>
	}

	Polish = [Ordered] @{
		'Keyed/RimFridge' = [Ordered] @{
			'RimFridge.TargetTemperature' = 'Temperatura docelowa'
			'RimFridge.CurrentTemperature' = 'Aktualna temperatura'
			'RimFridge.Power' = 'Zasilanie'
			'RimFridge.RenameTheRefrigerator' = 'Zmień nazwę lodówki'
			'RimFridge.ToggleGlowColor' = 'Przełącz kolor podświetlenia'
			'RimFridge.ToggleGlowColorDesc' = 'Przełącz kolor podświetlenia między normalnym a ciemnym światłem.'
			'RimFridge.Compatibility' = 'Kompatybilność'
			'RimFridge.ForceApplicationOfThesePatches' = 'Wymuś zastosowanie tych poprawek dla innych wersji RimFridge.'
			'RimFridge.ModifyBasePowerRequirement' = 'Modyfikuj podstawowe zapotrzebowanie na energię'
			'RimFridge.Apply' = 'Zastosuj'
			'RimFridge.PowerFactorExplanation' = 'Nowe zużycie energii = Wprowadzona wartość × Oryginalne zużycie energii'
			'RimFridge.BasePowerFactor' = 'Współczynnik podstawowego zużycia energii'
			'RimFridge.CannotBeLessThanOrEqualToZero' = '{0} nie może być mniejsze lub równe 0.'
			'RimFridge.UnableToParseToANumber' = 'Nie można przekształcić {0} na liczbę.'
			'RimFridge.NewPowerFactorApplied' = 'Zastosowano nowy współczynnik zużycia energii'
			'RimFridge.ActAsTradeBeacon' = 'Działaj jako latarnia handlowa'
			'RimFridge.FrostyBeverage' = 'Mroźny Napój'
			'RimFridge.EnableFrostyBeverages' = 'Włącz Mroźne Napoje'
		} <# END Keyed/RimFridge #>
		'DefInjected/ThingDef/Fridge_Building' = [Ordered] @{
			'RimFridge_Refrigerator.label' = 'Chłodziarka'
			'RimFridge_Refrigerator.description' = 'Łatwo psujące się towary nie gniją w tej chłodziarce'
			'RimFridge_SingleRefrigerator.label' = 'Mała chłodziarka'
			'RimFridge_SingleRefrigerator.description' = 'Łatwo psujące się towary nie gniją w tej małej chłodziarce'
			'RimFridge_QuadRefrigerator.label' = 'Duża chłodziarka'
			'RimFridge_QuadRefrigerator.description' = 'Łatwo psujące się towary nie gniją w tej dużej chłodziarce'
			'RimFridge_SingleRefrigerator.building.groupingLabel' = 'Mała chłodziarka'
			'RimFridge_Refrigerator.building.groupingLabel' = 'Chłodziarka'
			'RimFridge_QuadRefrigerator.building.groupingLabel' = 'Duża chłodziarka'
		} <# END DefInjected/ThingDef/Fridge_Building #>
		'DefInjected/ThingDef/WallFridge_Building' = [Ordered] @{
			'RimFridge_SingleWallRefrigerator.building.groupingLabel' = 'Ściana Pojedyncza Lodówka'
			'RimFridge_WallRefrigerator.building.groupingLabel' = 'Podwójna Lodówka Ścienna'
		} <# END DefInjected/ThingDef/WallFridge_Building #>
		'DefInjected/ThoughtDef/FrostyBeer' = [Ordered] @{
			'FrostyBeer.stages.0.label' = 'Cieszy się z zimnego browaru'
			'FrostyBeer.stages.0.description' = 'Niema nic lepszego niż chłodny napój!'
		} <# END DefInjected/ThoughtDef/FrostyBeer #>
	}

	Portuguese = [Ordered] @{
		'Keyed/RimFridge' = [Ordered] @{
			'RimFridge.TargetTemperature' = 'Temperatura Alvo'
			'RimFridge.CurrentTemperature' = 'Temperatura Atual'
			'RimFridge.Power' = 'Energia'
			'RimFridge.RenameTheRefrigerator' = 'Renomear o Refrigerador'
			'RimFridge.ToggleGlowColor' = 'Alternar Cor do Brilho'
			'RimFridge.ToggleGlowColorDesc' = 'Alterna a cor do brilho entre luz normal e luz escura.'
			'RimFridge.Compatibility' = 'Compatibilidade'
			'RimFridge.ForceApplicationOfThesePatches' = 'Forçar a aplicação desses patches para outras versões do RimFridge.'
			'RimFridge.ModifyBasePowerRequirement' = 'Modificar Requisito Básico de Energia'
			'RimFridge.Apply' = 'Aplicar'
			'RimFridge.PowerFactorExplanation' = 'Novo Consumo de Energia = Valor Inserido × Consumo de Energia Original'
			'RimFridge.BasePowerFactor' = 'Fator de Energia Básico'
			'RimFridge.CannotBeLessThanOrEqualToZero' = '{0} não pode ser menor ou igual a 0.'
			'RimFridge.UnableToParseToANumber' = 'Não foi possível converter {0} para um número.'
			'RimFridge.NewPowerFactorApplied' = 'Novo Fator de Energia Aplicado'
			'RimFridge.ActAsTradeBeacon' = 'Atuar como Farol de Comércio'
			'RimFridge.FrostyBeverage' = 'Bebida Gelada'
			'RimFridge.EnableFrostyBeverages' = 'Habilitar Bebidas Geladas'
		} <# END Keyed/RimFridge #>
		'DefInjected/ThingDef/Fridge_Building' = [Ordered] @{
			'RimFridge_Refrigerator.label' = 'Refrigerador'
			'RimFridge_Refrigerator.description' = 'Coisas que estragam guardadas aqui não apodrecerão'
			'RimFridge_SingleRefrigerator.label' = 'Refrigerador Único'
			'RimFridge_SingleRefrigerator.description' = 'Coisas que estragam guardadas aqui não apodrecerão'
			'RimFridge_SingleRefrigerator.building.groupingLabel' = 'Refrigerador Único'
			'RimFridge_Refrigerator.building.groupingLabel' = 'Refrigerador'
			'RimFridge_QuadRefrigerator.building.groupingLabel' = ''
		} <# END DefInjected/ThingDef/Fridge_Building #>
		'DefInjected/ThingDef/WallFridge_Building' = [Ordered] @{
			'RimFridge_SingleWallRefrigerator.building.groupingLabel' = 'Único Refrigerador Da Parede'
			'RimFridge_WallRefrigerator.building.groupingLabel' = 'Refrigerador Duplo Da Parede'
		} <# END DefInjected/ThingDef/WallFridge_Building #>
		'DefInjected/ThoughtDef/FrostyBeer' = [Ordered] @{
			'FrostyBeer.stages.0.label' = 'curtiu uma gelada'
			'FrostyBeer.stages.0.description' = 'Nada melhor do que uma bebida gelada!'
		} <# END DefInjected/ThoughtDef/FrostyBeer #>
	}

	PortugueseBrazilian = [Ordered] @{
		'Keyed/RimFridge' = [Ordered] @{
			'RimFridge.TargetTemperature' = 'Temperatura Alvo'
			'RimFridge.CurrentTemperature' = 'Temperatura Atual'
			'RimFridge.Power' = 'Energia'
			'RimFridge.RenameTheRefrigerator' = 'Renomear o Refrigerador'
			'RimFridge.ToggleGlowColor' = 'Alternar Cor do Brilho'
			'RimFridge.ToggleGlowColorDesc' = 'Altere a cor do brilho entre luz normal e luz escura.'
			'RimFridge.Compatibility' = 'Compatibilidade'
			'RimFridge.ForceApplicationOfThesePatches' = 'Forçar a aplicação desses patches para outras versões do RimFridge.'
			'RimFridge.ModifyBasePowerRequirement' = 'Modificar Requisito Básico de Energia'
			'RimFridge.Apply' = 'Aplicar'
			'RimFridge.PowerFactorExplanation' = 'Novo Consumo de Energia = Valor Inserido × Consumo de Energia Original'
			'RimFridge.BasePowerFactor' = 'Fator de Energia Base'
			'RimFridge.CannotBeLessThanOrEqualToZero' = '{0} não pode ser menor ou igual a 0.'
			'RimFridge.UnableToParseToANumber' = 'Não foi possível converter {0} para um número.'
			'RimFridge.NewPowerFactorApplied' = 'Novo Fator de Energia Aplicado'
			'RimFridge.ActAsTradeBeacon' = 'Atuar como Sinalizador de Comércio'
			'RimFridge.FrostyBeverage' = 'Bebida Gelada'
			'RimFridge.EnableFrostyBeverages' = 'Habilitar Bebidas Geladas'
		} <# END Keyed/RimFridge #>
		'DefInjected/ThingDef/Fridge_Building' = [Ordered] @{
			'RimFridge_Refrigerator.label' = 'Geladeira (2x1)'
			'RimFridge_Refrigerator.description' = 'Armazena e congela refeições (entre outras coisas) para evitar que estrague.'
			'RimFridge_Refrigerator_Blueprint.label' = 'Dual Refrigerator (blueprint)'
			'RimFridge_Refrigerator_Blueprint_Install.label' = 'Dual Refrigerator (blueprint)'
			'RimFridge_Refrigerator_Frame.label' = 'Dual Refrigerator (building)'
			'RimFridge_Refrigerator_Frame.description' = 'Armazena e congela refeições (entre outras coisas) para evitar que estrague.'
			'RimFridge_SingleRefrigerator.label' = 'Minibar (1x1)'
			'RimFridge_SingleRefrigerator.description' = 'Um pequeno refrigerador para guardar comida e outras coisas.'
			'RimFridge_SingleRefrigerator_Blueprint.label' = 'Minibar (blueprint)'
			'RimFridge_SingleRefrigerator_Blueprint_Install.label' = 'Minibar (blueprint)'
			'RimFridge_SingleRefrigerator_Frame.label' = 'Minibar (construção)'
			'RimFridge_SingleRefrigerator_Frame.description' = 'Um pequeno refrigerador para guardar comida e outras coisas'
			'RimFridge_QuadRefrigerator.label' = 'Refrigerador quadrado (2x2)'
			'RimFridge_QuadRefrigerator.description' = 'Um grande frizer para armazenar e congelar alimentos'
			'RimFridge_QuadRefrigerator_Blueprint.label' = 'Refrigerador quadrado (blueprint)'
			'RimFridge_QuadRefrigerator_Blueprint_Install.label' = 'Refrigerador quadrado (blueprint)'
			'RimFridge_QuadRefrigerator_Frame.label' = 'Refrigerador quadrado (construção)'
			'RimFridge_QuadRefrigerator_Frame.description' = 'Um grande frizer para armazenar e congelar alimentos'
			'RimFridge_SingleRefrigerator.building.groupingLabel' = 'Minibar (1x1)'
			'RimFridge_Refrigerator.building.groupingLabel' = 'Geladeira (2x1)'
			'RimFridge_QuadRefrigerator.building.groupingLabel' = 'Refrigerador quadrado (2x2)'
		} <# END DefInjected/ThingDef/Fridge_Building #>
		'DefInjected/ThingDef/WallFridge_Building' = [Ordered] @{
			'RimFridge_WallRefrigerator.label' = 'Geladeira de parede (2x1)'
			'RimFridge_WallRefrigerator.description' = 'Armazena e congela refeições (entre outras coisas) para evitar que estrague.'
			'RimFridge_WallRefrigerator_Blueprint.label' = 'Geladeira de parede (2x1) (blueprint)'
			'RimFridge_WallRefrigerator_Blueprint_Install.label' = 'Geladeira de parede (2x1) (blueprint)'
			'RimFridge_WallRefrigerator_Frame.label' = 'Geladeira de parede (2x1) (construção)'
			'RimFridge_WallRefrigerator_Frame.description' = 'Armazena e congela refeições (entre outras coisas) para evitar que estrague.'
			'RimFridge_SingleWallRefrigerator.label' = 'Minibar de parede (1x1)'
			'RimFridge_SingleWallRefrigerator.description' = 'Um pequeno refrigerador para guardar comida e outras coisas.'
			'RimFridge_SingleWallRefrigerator_Blueprint.label' = 'Minibar de parede (1x1) (blueprint)'
			'RimFridge_SingleWallRefrigerator_Blueprint_Install.label' = 'Minibar de parede (1x1) (blueprint)'
			'RimFridge_SingleWallRefrigerator_Frame.label' = 'Minibar de parede (1x1) (construção)'
			'RimFridge_SingleWallRefrigerator_Frame.description' = 'Um pequeno refrigerador para guardar comida e outras coisas'
			'RimFridge_SingleWallRefrigerator.building.groupingLabel' = 'Minibar de parede (1x1)'
			'RimFridge_WallRefrigerator.building.groupingLabel' = 'Geladeira de parede (2x1)'
		} <# END DefInjected/ThingDef/WallFridge_Building #>
		'DefInjected/ThoughtDef/FrostyBeer' = [Ordered] @{
			'FrostyBeer.stages.0.label' = 'Bebida gelada'
			'FrostyBeer.stages.0.description' = 'Não há nada melhor que uma bebida gelada!'
		} <# END DefInjected/ThoughtDef/FrostyBeer #>
	}

	Russian = [Ordered] @{
		'Keyed/RimFridge' = [Ordered] @{
			'RimFridge.TargetTemperature' = 'Целевая температура'
			'RimFridge.CurrentTemperature' = 'Текущая температура'
			'RimFridge.Power' = 'Энергия'
			'RimFridge.RenameTheRefrigerator' = 'Переименовать холодильник'
			'RimFridge.ToggleGlowColor' = 'Переключить цвет свечения'
			'RimFridge.ToggleGlowColorDesc' = 'Переключение цвета свечения между обычным и темным светом.'
			'RimFridge.Compatibility' = 'Совместимость'
			'RimFridge.ForceApplicationOfThesePatches' = 'Принудительное применение этих патчей для других версий RimFridge.'
			'RimFridge.ModifyBasePowerRequirement' = 'Изменить базовое потребление энергии'
			'RimFridge.Apply' = 'Применить'
			'RimFridge.PowerFactorExplanation' = 'Новое потребление энергии = Введенное значение × Исходное потребление энергии'
			'RimFridge.BasePowerFactor' = 'Базовый коэффициент энергии'
			'RimFridge.CannotBeLessThanOrEqualToZero' = '{0} не может быть меньше или равно 0.'
			'RimFridge.UnableToParseToANumber' = 'Не удалось преобразовать {0} в число.'
			'RimFridge.NewPowerFactorApplied' = 'Новый коэффициент энергии применен'
			'RimFridge.ActAsTradeBeacon' = 'Работать как торговый маяк'
			'RimFridge.FrostyBeverage' = 'Замороженный напиток'
			'RimFridge.EnableFrostyBeverages' = 'Влончские замороженные напитки'
		} <# END Keyed/RimFridge #>
		'DefInjected/ThingDef/Fridge_Building' = [Ordered] @{
			'RimFridge_Refrigerator.label' = 'холодильник (две камеры)'
			'RimFridge_Refrigerator.description' = 'Бытовой холодильник, использующийся для хранения скоропортящихся продуктов.'
			'RimFridge_Refrigerator_Blueprint.label' = 'двухкамерный холодильник (проект)'
			'RimFridge_Refrigerator_Blueprint_Install.label' = 'двухкамерный холодильник (проект)'
			'RimFridge_Refrigerator_Frame.label' = 'двухкамерный холодильник (строится)'
			'RimFridge_Refrigerator_Frame.description' = 'Бытовой холодильник, использующийся для хранения скоропортящихся продуктов.'
			'RimFridge_SingleRefrigerator.label' = 'холодильник (одна камера)'
			'RimFridge_SingleRefrigerator.description' = 'Компактный бытовой холодильник, использующийся для хранения скоропортящихся продуктов.'
			'RimFridge_SingleRefrigerator_Blueprint.label' = 'однокамерный холодильник (проект)'
			'RimFridge_SingleRefrigerator_Blueprint_Install.label' = 'однокамерный холодильник (проект)'
			'RimFridge_SingleRefrigerator_Frame.label' = 'однокамерный холодильник (строится)'
			'RimFridge_SingleRefrigerator_Frame.description' = 'Компактный бытовой холодильник, использующийся для хранения скоропортящихся продуктов.'
			'RimFridge_QuadRefrigerator.label' = 'холодильник (четыре камеры)'
			'RimFridge_QuadRefrigerator.description' = 'Вместительный холодильник, использующийся для хранения скоропортящихся продуктов. Такое оборудование обычно устанавливают на крупных производствах.'
			'RimFridge_QuadRefrigerator_Blueprint.label' = 'четырехкамерный холодильник (проект)'
			'RimFridge_QuadRefrigerator_Blueprint_Install.label' = 'четырехкамерный холодильник (проект)'
			'RimFridge_QuadRefrigerator_Frame.label' = 'четырехкамерный холодильник (строится)'
			'RimFridge_QuadRefrigerator_Frame.description' = 'Вместительный холодильник, использующийся для хранения скоропортящихся продуктов. Такое оборудование обычно устанавливают на крупных производствах.'
			'RimFridge_SingleRefrigerator.building.groupingLabel' = 'холодильник (одна камера)'
			'RimFridge_Refrigerator.building.groupingLabel' = 'холодильник (две камеры)'
			'RimFridge_QuadRefrigerator.building.groupingLabel' = 'холодильник (четыре камеры)'
		} <# END DefInjected/ThingDef/Fridge_Building #>
		'DefInjected/ThingDef/WallFridge_Building' = [Ordered] @{
			'RimFridge_WallRefrigerator.label' = 'встроенный холодильник (две камеры)'
			'RimFridge_WallRefrigerator.description' = 'Бытовой холодильник, встроенный в стену. Может быть использован как стена, разделяет помещения.'
			'RimFridge_WallRefrigerator_Blueprint.label' = 'встроенный двухкамерный холодильник (проект)'
			'RimFridge_WallRefrigerator_Blueprint_Install.label' = 'встроенный двухкамерный холодильник (проект)'
			'RimFridge_WallRefrigerator_Frame.label' = 'встроенный двухкамерный холодильник (строится)'
			'RimFridge_WallRefrigerator_Frame.description' = 'Бытовой холодильник, встроенный в стену. Может быть использован как стена, разделяет помещения.'
			'RimFridge_SingleWallRefrigerator.label' = 'встроенный холодильник (одна камера)'
			'RimFridge_SingleWallRefrigerator.description' = 'Бытовой холодильник, встроенный в стену. Может быть использован как стена, разделяет помещения.'
			'RimFridge_SingleWallRefrigerator_Blueprint.label' = 'встроенный однокамерный холодильник (проект)'
			'RimFridge_SingleWallRefrigerator_Blueprint_Install.label' = 'встроенный однокамерный холодильник (проект)'
			'RimFridge_SingleWallRefrigerator_Frame.label' = 'встроенный однокамерный холодильник (строится)'
			'RimFridge_SingleWallRefrigerator_Frame.description' = 'Бытовой холодильник, встроенный в стену. Может быть использован как стена, разделяет помещения.'
			'RimFridge_SingleWallRefrigerator.building.groupingLabel' = 'встроенный холодильник (одна камера)'
			'RimFridge_WallRefrigerator.building.groupingLabel' = 'встроенный холодильник (две камеры)'
		} <# END DefInjected/ThingDef/WallFridge_Building #>
		'DefInjected/ThoughtDef/FrostyBeer' = [Ordered] @{
			'FrostyBeer.stages.0.label' = 'насладиться холодненьким'
			'FrostyBeer.stages.0.description' = 'Нет ничего лучше холодного напитка!'
		} <# END DefInjected/ThoughtDef/FrostyBeer #>
	}

	Spanish = [Ordered] @{
		'Keyed/RimFridge' = [Ordered] @{
			'RimFridge.TargetTemperature' = 'Temperatura Objetivo'
			'RimFridge.CurrentTemperature' = 'Temperatura Actual'
			'RimFridge.Power' = 'Energía'
			'RimFridge.RenameTheRefrigerator' = 'Renombrar el Refrigerador'
			'RimFridge.ToggleGlowColor' = 'Cambiar el Color del Brillo'
			'RimFridge.ToggleGlowColorDesc' = 'Cambia el color del brillo entre luz normal y luz tenue.'
			'RimFridge.Compatibility' = 'Compatibilidad'
			'RimFridge.ForceApplicationOfThesePatches' = 'Forzar la aplicación de estos parches para otras versiones de RimFridge.'
			'RimFridge.ModifyBasePowerRequirement' = 'Modificar el Requisito de Energía Base'
			'RimFridge.Apply' = 'Aplicar'
			'RimFridge.PowerFactorExplanation' = 'Nuevo Consumo de Energía = Valor de Entrada × Consumo de Energía Original'
			'RimFridge.BasePowerFactor' = 'Factor de Energía Base'
			'RimFridge.CannotBeLessThanOrEqualToZero' = '{0} no puede ser menor o igual a 0.'
			'RimFridge.UnableToParseToANumber' = 'No se pudo convertir {0} en un número.'
			'RimFridge.NewPowerFactorApplied' = 'Nuevo Factor de Energía Aplicado'
			'RimFridge.ActAsTradeBeacon' = 'Actuar como un Faro Comercial'
			'RimFridge.FrostyBeverage' = 'Bebidas heladas'
			'RimFridge.EnableFrostyBeverages' = 'Habilitar bebidas heladas'
		} <# END Keyed/RimFridge #>
		'DefInjected/ThingDef/Fridge_Building' = [Ordered] @{
			'RimFridge_Refrigerator.label' = 'Refrigerador doble'
			'RimFridge_Refrigerator.description' = 'Las cosas perecederas almacenadas aquí no se deteriorarán.'
			'RimFridge_SingleRefrigerator.label' = 'Refrigerador'
			'RimFridge_SingleRefrigerator.description' = 'Las cosas perecederas almacenadas aquí no se deteriorarán.'
			'RimFridge_QuadRefrigerator.label' = 'Refrigerador cuadruple'
			'RimFridge_QuadRefrigerator.description' = 'Las cosas perecederas almacenadas aquí no se deteriorarán.'
			'RimFridge_SingleRefrigerator.building.groupingLabel' = 'Refrigerador'
			'RimFridge_Refrigerator.building.groupingLabel' = 'Refrigerador doble'
			'RimFridge_QuadRefrigerator.building.groupingLabel' = 'Refrigerador cuadruple'
		} <# END DefInjected/ThingDef/Fridge_Building #>
		'DefInjected/ThingDef/WallFridge_Building' = [Ordered] @{
			'RimFridge_WallRefrigerator.label' = 'Refrigerador-Muro doble'
			'RimFridge_WallRefrigerator.description' = 'Las cosas perecederas almacenadas aquí no se deteriorarán.'
			'RimFridge_SingleWallRefrigerator.label' = 'Refrigerador-Muro'
			'RimFridge_SingleWallRefrigerator.description' = 'Las cosas perecederas almacenadas aquí no se deteriorarán.'
			'RimFridge_SingleWallRefrigerator.building.groupingLabel' = 'Refrigerador-Muro'
			'RimFridge_WallRefrigerator.building.groupingLabel' = 'Refrigerador-Muro doble'
		} <# END DefInjected/ThingDef/WallFridge_Building #>
		'DefInjected/ThoughtDef/FrostyBeer' = [Ordered] @{
			'FrostyBeer.stages.0.label' = 'Disfruté de una bebida fría'
			'FrostyBeer.stages.0.description' = '¡Nada mejor que una bebida fría!'
		} <# END DefInjected/ThoughtDef/FrostyBeer #>
	}

	SpanishLatin = [Ordered] @{
		'Keyed/RimFridge' = [Ordered] @{
			'RimFridge.TargetTemperature' = 'Temperatura Objetivo'
			'RimFridge.CurrentTemperature' = 'Temperatura Actual'
			'RimFridge.Power' = 'Energía'
			'RimFridge.RenameTheRefrigerator' = 'Renombrar el Refrigerador'
			'RimFridge.ToggleGlowColor' = 'Cambiar el Color del Brillo'
			'RimFridge.ToggleGlowColorDesc' = 'Cambia el color del brillo entre luz normal y luz tenue.'
			'RimFridge.Compatibility' = 'Compatibilidad'
			'RimFridge.ForceApplicationOfThesePatches' = 'Forzar la aplicación de estos parches para otras versiones de RimFridge.'
			'RimFridge.ModifyBasePowerRequirement' = 'Modificar el Requisito de Energía Base'
			'RimFridge.Apply' = 'Aplicar'
			'RimFridge.PowerFactorExplanation' = 'Nuevo Consumo de Energía = Valor de Entrada × Consumo de Energía Original'
			'RimFridge.BasePowerFactor' = 'Factor de Energía Base'
			'RimFridge.CannotBeLessThanOrEqualToZero' = '{0} no puede ser menor o igual a 0.'
			'RimFridge.UnableToParseToANumber' = 'No se pudo convertir {0} en un número.'
			'RimFridge.NewPowerFactorApplied' = 'Nuevo Factor de Energía Aplicado'
			'RimFridge.ActAsTradeBeacon' = 'Actuar como un Faro Comercial'
			'RimFridge.FrostyBeverage' = 'Bebidas heladas'
			'RimFridge.EnableFrostyBeverages' = 'Habilitar bebidas heladas'
		} <# END Keyed/RimFridge #>
		'DefInjected/ThingDef/Fridge_Building' = [Ordered] @{
			'RimFridge_Refrigerator.label' = 'Refrigerador doble'
			'RimFridge_Refrigerator.description' = 'Las cosas perecederas almacenadas aquí no se deteriorarán.'
			'RimFridge_SingleRefrigerator.label' = 'Refrigerador'
			'RimFridge_SingleRefrigerator.description' = 'Las cosas perecederas almacenadas aquí no se deteriorarán.'
			'RimFridge_QuadRefrigerator.label' = 'Refrigerador cuadruple'
			'RimFridge_QuadRefrigerator.description' = 'Las cosas perecederas almacenadas aquí no se deteriorarán.'
			'RimFridge_SingleRefrigerator.building.groupingLabel' = 'Refrigerador'
			'RimFridge_Refrigerator.building.groupingLabel' = 'Refrigerador doble'
			'RimFridge_QuadRefrigerator.building.groupingLabel' = 'Refrigerador cuadruple'
		} <# END DefInjected/ThingDef/Fridge_Building #>
		'DefInjected/ThingDef/WallFridge_Building' = [Ordered] @{
			'RimFridge_WallRefrigerator.label' = 'Refrigerador-Muro doble'
			'RimFridge_WallRefrigerator.description' = 'Las cosas perecederas almacenadas aquí no se deteriorarán.'
			'RimFridge_SingleWallRefrigerator.label' = 'Refrigerador-Muro'
			'RimFridge_SingleWallRefrigerator.description' = 'Las cosas perecederas almacenadas aquí no se deteriorarán.'
			'RimFridge_SingleWallRefrigerator.building.groupingLabel' = 'Refrigerador-Muro'
			'RimFridge_WallRefrigerator.building.groupingLabel' = 'Refrigerador-Muro doble'
		} <# END DefInjected/ThingDef/WallFridge_Building #>
		'DefInjected/ThoughtDef/FrostyBeer' = [Ordered] @{
			'FrostyBeer.stages.0.label' = 'Disfruté de una bebida fría'
			'FrostyBeer.stages.0.description' = '¡Nada mejor que una bebida fría!'
		} <# END DefInjected/ThoughtDef/FrostyBeer #>
	}
}


[PSCustomObject] @{
	Translations = $Translations
}

