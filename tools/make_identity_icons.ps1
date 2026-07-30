param(
    [string]$Root = 'F:\Programs\SlayTheSpire2-SmashFromFGO',
    [switch]$RefreshSources
)

Add-Type -AssemblyName System.Drawing
$ErrorActionPreference = 'Stop'

function Source([string]$url, [string]$label) {
    @{ Url = $url; Label = $label }
}

$characters = @(
    @{
        Project = 'MashShielder'; Name = 'Mash Kyrielight'; ServantId = '800100'
        Sources = @{
            face0 = Source 'https://static.atlasacademy.io/JP/Faces/f_8001000.png' 'Mash Kyrielight ascension face 1'
            camelot = Source 'https://static.atlasacademy.io/JP/Servants/Commands/800100/card_servant_np1.png' 'Lord Camelot command card'
            home = Source 'https://static.atlasacademy.io/JP/EquipFaces/f_98098600.png' 'Chaldea Home Sweet Home Valentine CE'
        }
        Powers = @{
            lord_camelot_charge_power='camelot'
        }
        Relics = @{
            a_team_diary='home'; spare_glasses='face0'
        }
    },
    @{
        Project = 'MordredSaber'; Name = 'Mordred'; ServantId = '100900'
        Sources = @{
            face0 = Source 'https://static.atlasacademy.io/JP/Faces/f_1009000.png' 'Mordred ascension face 1'
            face2 = Source 'https://static.atlasacademy.io/JP/Faces/f_1009002.png' 'Mordred ascension face 3'
            face3 = Source 'https://static.atlasacademy.io/JP/Faces/f_1009003.png' 'Mordred final ascension face'
            mana = Source 'https://static.atlasacademy.io/JP/SkillIcons/skill_00306.png' 'Mana Burst A / Knight of Red Lightning A+'
            intuition = Source 'https://static.atlasacademy.io/JP/SkillIcons/skill_00603.png' 'Intuition B'
            secret = Source 'https://static.atlasacademy.io/JP/SkillIcons/skill_00400.png' 'Secret of Pedigree EX'
            cigarette = Source 'https://static.atlasacademy.io/JP/SkillIcons/skill_00308.png' 'Cigarette Lion B+'
            np = Source 'https://static.atlasacademy.io/JP/Servants/Commands/100900/card_servant_np.png' 'Clarent Blood Arthur command card'
            bond = Source 'https://static.atlasacademy.io/JP/EquipFaces/f_93005100.png' 'Who Am I? bond CE'
            valentine = Source 'https://static.atlasacademy.io/JP/EquipFaces/f_98016000.png' 'Strawberry Thunder Crunch Valentine CE'
        }
        Powers = @{
            masked_knight_form_power='face0'; rebellion_form_power='face2'; crimson_lightning_form_power='face3'
            red_lightning_channel_power='mana'; crit_consumed_this_turn_power='intuition'; clarent_manifested_power='np'
            lightning_visit_return_power='mana'; crown_of_lightning_power='mana'; ambition_for_throne_power='secret'
            memory_of_trifas_power='bond'; the_most_radiant_sword_power='np'; saberface_power='face2'
            double_edge_of_hatred_power='face3'; secret_revealed_power='secret'; camlann_guts_power='face3'
            knight_of_red_lightning_power='mana'; cigarette_lion_power='cigarette'; dragons_blood_power='mana'
            accumulated_hatred_power='face3'; banner_of_rebellion_power='bond'; homunculus_acceleration_power='intuition'
            red_lightning_spark_power='mana'
        }
        Relics = @{
            clarent_the_stolen_sword='np'; clarent_overloaded_with_hatred='np'; oath_of_the_knight_of_treachery='bond'
            summoning_seal_saber_of_red='face2'; red_bike_of_trifas='face2'; kairis_cigarettes='cigarette'
            magic_resistance_b_charm='secret'; red_glasses_of_saber='face2'; banner_of_camlann='bond'
            the_empty_seat_of_the_round_table='bond'; grey_cat_of_trifas='valentine'; holy_grail_of_selection='bond'
        }
    },
    @{
        Project = 'GilgameshArcher'; Name = 'Gilgamesh'; ServantId = '200200'
        Sources = @{
            face0 = Source 'https://static.atlasacademy.io/JP/Faces/f_2002000.png' 'Gilgamesh ascension face 1'
            face2 = Source 'https://static.atlasacademy.io/JP/Faces/f_2002002.png' 'Gilgamesh ascension face 3'
            face3 = Source 'https://static.atlasacademy.io/JP/Faces/f_2002003.png' 'Gilgamesh final ascension face'
            charisma = Source 'https://static.atlasacademy.io/JP/SkillIcons/skill_00300.png' 'Charisma A+ / He Who Saw the Deep EX'
            golden = Source 'https://static.atlasacademy.io/JP/SkillIcons/skill_00602.png' 'Golden Rule A'
            collector = Source 'https://static.atlasacademy.io/JP/SkillIcons/skill_00311.png' 'Collector EX / Treasury of Babylon EX'
            np = Source 'https://static.atlasacademy.io/JP/Servants/Commands/200200/card_servant_np.png' 'Enuma Elish command card'
            bond = Source 'https://static.atlasacademy.io/JP/EquipFaces/f_93002100.png' "Key of the King's Law bond CE"
            valentine = Source 'https://static.atlasacademy.io/JP/EquipFaces/f_98051200.png' 'Lapis Lazuli Bracelet Valentine CE'
        }
        Powers = @{
            divinity_power='face2'; independent_action_power='charisma'; arms_played_power='collector'
            enuma_manifested_power='np'; cards_this_turn_power='collector'; throne_of_the_onlooker_power='face3'
            golden_arrogance_power='golden'; recital_of_creation_power='np'; kings_arrogance_power='charisma'
            treasure_power='collector'
        }
        Relics = @{
            bab_ilu='bond'; oath_of_uruk='bond'; catalog_of_the_royal_treasury='collector'
            kings_wine_cup='valentine'; mantle_of_arrogance='face3'; vimana_golden_throne='np'
            magic_resistance_amulet='face0'; the_original_chalice='bond'
        }
    },
    @{
        Project = 'OkitaSaber'; Name = 'Okita Souji'; ServantId = '102700'
        Sources = @{
            face0 = Source 'https://static.atlasacademy.io/JP/Faces/f_1027000.png' 'Okita ascension face 1'
            face2 = Source 'https://static.atlasacademy.io/JP/Faces/f_1027002.png' 'Okita ascension face 3'
            face3 = Source 'https://static.atlasacademy.io/JP/Faces/f_1027003.png' 'Okita final ascension face'
            shukuchi = Source 'https://static.atlasacademy.io/JP/SkillIcons/skill_00304.png' 'Shukuchi B/B+'
            weakness = Source 'https://static.atlasacademy.io/JP/SkillIcons/skill_00311.png' 'Weak Constitution A / Zettou A'
            mindeye = Source 'https://static.atlasacademy.io/JP/SkillIcons/skill_00402.png' "Mind's Eye (Fake) A"
            np = Source 'https://static.atlasacademy.io/JP/Servants/Commands/102700/card_servant_np.png' 'Mumyou Sandanzuki command card'
            bond = Source 'https://static.atlasacademy.io/JP/EquipFaces/f_93004200.png' 'Headband of Resolve bond CE'
            valentine = Source 'https://static.atlasacademy.io/JP/EquipFaces/f_98015000.png' 'Yatsuhashi Chocolate Valentine CE'
        }
        Powers = @{
            aliento_power='mindeye'; attacks_this_turn_power='weakness'; swift_stance_power='shukuchi'
            glory_edge_power='weakness'; sword_genius_power='weakness'; makoto_power='bond'
            prodigy_sense_power='mindeye'; thousand_thrusts_power='np'; shinsengumi_will_power='bond'
            late_bloom_power='weakness'; steady_step_power='mindeye'; riding_e_power='shukuchi'
            last_spring_memory_power='bond'; tennen_rishin_breath_power='mindeye'; bakumatsu_spirit_power='bond'
            to_the_end_power='face3'; event_saturation_power='face0'; bakumatsu_flower_power='valentine'
            mumyou_manifested_power='np'
        }
        Relics = @{
            haori_asagi='bond'; bond_first_unit='bond'; menkyo_kaiden='bond'; three_color_dango='valentine'
            gudaguda_poster='face0'; kiku_ichimonji_norimune='np'; sakura_petals='valentine'
            dr_matsumoto_medicine='weakness'; makoto_banner='bond'; first_unit_badge='bond'
            flower_of_imperial_capital='valentine'; holy_grail_imperial_capital='np'
        }
    },
    @{
        Project = 'OberonPretender'; Name = 'Oberon'; ServantId = '2800100'
        Sources = @{
            face0 = Source 'https://static.atlasacademy.io/JP/Faces/f_28001000.png' 'Oberon ascension face 1'
            face1 = Source 'https://static.atlasacademy.io/JP/Faces/f_28001001.png' 'Oberon ascension face 2'
            face3 = Source 'https://static.atlasacademy.io/JP/Faces/f_28001003.png' 'Oberon final ascension face'
            evening = Source 'https://static.atlasacademy.io/JP/SkillIcons/skill_00302.png' 'Evening Shroud EX'
            morning = Source 'https://static.atlasacademy.io/JP/SkillIcons/skill_00601.png' 'Morning Lark EX'
            ending = Source 'https://static.atlasacademy.io/JP/SkillIcons/skill_00306.png' 'Ending of Dreams EX'
            np = Source 'https://static.atlasacademy.io/JP/Servants/Commands/2800100/card_servant_np.png' 'Rye Rhyme Goodfellow command card'
            bond = Source 'https://static.atlasacademy.io/JP/EquipFaces/f_93061700.png' 'Pavane Pour Une Infante Defunte bond CE'
            valentine = Source 'https://static.atlasacademy.io/JP/EquipFaces/f_98081300.png' "Void's Dust Valentine CE"
        }
        Powers = @{
            debt_power='valentine'; interest_in_my_favor_power='valentine'; nocturnal_euphoria_power='evening'
            insects_vigil_power='evening'; luck_ex_power='morning'; night_reading_power='face0'
            happy_ending_plan_power='bond'; threat_to_humanity_power='face3'; vespers_of_the_end_power='evening'
            wings_of_reverie_power='face0'; storybook_king_power='face0'; winter_prince_power='face1'
            vortigern_power='face3'; sleep_power='ending'; insomnia_power='evening'; item_construction_power='morning'
            ult_manifested_power='np'; court_of_insect_fae_power='bond'; evening_shroud_power='evening'
            ending_of_dreams_power='ending'; no_draw_next_turn_power='ending'
        }
        Relics = @{
            dream_contract='bond'; chronicle_of_avalon='bond'; forget_me_not_of_autumn_wood='face0'
            book_of_dreams_end='bond'; usurers_purse='valentine'; hawk_moth='face0'
            princes_flower_crown='face1'; midsummer_nights_dream_ex='bond'; feather_of_the_contract='face0'
            clock_of_dawn='morning'; holy_grail_of_the_fae='np'
        }
    },
    @{
        Project = 'SiegfriedSaber'; Name = 'Siegfried'; ServantId = '100800'
        Sources = @{
            face0 = Source 'https://static.atlasacademy.io/JP/Faces/f_1008000.png' 'Siegfried ascension face 1'
            face2 = Source 'https://static.atlasacademy.io/JP/Faces/f_1008002.png' 'Siegfried ascension face 3'
            face3 = Source 'https://static.atlasacademy.io/JP/Faces/f_1008003.png' 'Siegfried final ascension face'
            golden = Source 'https://static.atlasacademy.io/JP/SkillIcons/skill_00602.png' 'Golden Rule C-'
            disengage = Source 'https://static.atlasacademy.io/JP/SkillIcons/skill_00608.png' 'Disengage A'
            dragon = Source 'https://static.atlasacademy.io/JP/SkillIcons/skill_00301.png' 'Dragon-Slayer A/A++'
            avarice = Source 'https://static.atlasacademy.io/JP/SkillIcons/skill_00601.png' 'Avaricious Gold A'
            np = Source 'https://static.atlasacademy.io/JP/Servants/Commands/100800/card_servant_np.png' 'Balmung command card'
            bond = Source 'https://static.atlasacademy.io/JP/EquipFaces/f_93002400.png' 'Das Rheingold bond CE'
            valentine = Source 'https://static.atlasacademy.io/JP/EquipFaces/f_98050200.png' 'Dragon Cosplay Valentine CE'
        }
        Powers = @{
            exposed_back_power='face3'; linden_scar_power='bond'; weight_of_expectations_power='face2'
            maturing_scales_power='dragon'; baptism_of_fafnir_power='dragon'; peerless_crown_power='face2'
            tarnkappe_power='disengage'; siegfried_np_manifested_power='np'
        }
        Relics = @{
            linden_leaf='face3'; das_rheingold='bond'; fafnir_heartblood='dragon'; dragon_heart_scale='dragon'
            dragon_scale_aegis='dragon'; rhinegold_shard='avarice'; fafnirs_bane='np'
        }
    }
)

function Render-CircleIcon([string]$sourcePath, [string]$outputPath, [int]$size = 256) {
    $source = [System.Drawing.Bitmap]::FromFile($sourcePath)
    try {
        $side = [Math]::Min($source.Width, $source.Height)
        $sourceRect = New-Object System.Drawing.Rectangle(
            [int](($source.Width - $side) / 2), [int](($source.Height - $side) / 2), $side, $side)
        $output = New-Object System.Drawing.Bitmap($size, $size, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
        $graphics = [System.Drawing.Graphics]::FromImage($output)
        try {
            $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
            $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
            $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
            $clip = New-Object System.Drawing.Drawing2D.GraphicsPath
            $clip.AddEllipse(2, 2, $size - 4, $size - 4)
            $graphics.SetClip($clip)
            $graphics.DrawImage($source, (New-Object System.Drawing.Rectangle(0, 0, $size, $size)), $sourceRect, [System.Drawing.GraphicsUnit]::Pixel)
            $clip.Dispose()
        } finally { $graphics.Dispose() }
        $output.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
        $output.Dispose()
    } finally { $source.Dispose() }
}

function Render-Outline([string]$sourcePath, [string]$outputPath) {
    $source = [System.Drawing.Bitmap]::FromFile($sourcePath)
    try {
        $output = New-Object System.Drawing.Bitmap($source.Width, $source.Height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
        for ($y = 0; $y -lt $source.Height; $y++) {
            for ($x = 0; $x -lt $source.Width; $x++) {
                $alpha = $source.GetPixel($x, $y).A
                if ($alpha -gt 16) { $output.SetPixel($x, $y, [System.Drawing.Color]::FromArgb($alpha, 255, 255, 255)) }
            }
        }
        $output.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
        $output.Dispose()
    } finally { $source.Dispose() }
}

function Copy-WithRetry([string]$sourcePath, [string]$destinationPath) {
    for ($attempt = 1; $attempt -le 10; $attempt++) {
        try {
            Copy-Item $sourcePath $destinationPath -Force
            return
        } catch {
            if ($attempt -eq 10) { throw }
            Start-Sleep -Milliseconds 250
        }
    }
}

$manifest = [System.Collections.Generic.List[object]]::new()
foreach ($character in $characters) {
    $sourceDir = Join-Path $Root "assets\reference\icons\identity\$($character.Project)"
    $imageRoot = Join-Path $Root "$($character.Project)\$($character.Project)\images"
    New-Item -ItemType Directory -Force $sourceDir, "$imageRoot\powers\big", "$imageRoot\relics\big" | Out-Null

    foreach ($entry in $character.Sources.GetEnumerator()) {
        $sourcePath = Join-Path $sourceDir "$($entry.Key).png"
        if ($RefreshSources -or -not (Test-Path $sourcePath)) {
            Invoke-WebRequest -Uri $entry.Value.Url -OutFile $sourcePath
        }
    }

    foreach ($kind in 'Powers', 'Relics') {
        $folder = $kind.ToLowerInvariant()
        foreach ($entry in $character[$kind].GetEnumerator()) {
            $sourceKey = $entry.Value
            $source = $character.Sources[$sourceKey]
            $sourcePath = Join-Path $sourceDir "$sourceKey.png"
            $outputPath = Join-Path $imageRoot "$folder\$($entry.Key).png"
            Render-CircleIcon $sourcePath $outputPath
            Copy-WithRetry $outputPath (Join-Path $imageRoot "$folder\big\$($entry.Key).png")
            if ($kind -eq 'Relics') {
                Render-Outline $outputPath (Join-Path $imageRoot "$folder\$($entry.Key)_outline.png")
            }
            $manifest.Add([pscustomobject]@{
                Project = $character.Project
                Kind = $kind.TrimEnd('s')
                Asset = $entry.Key
                OfficialSource = $source.Label
                Url = $source.Url
            })
        }
    }
}

$manifest | Sort-Object Project,Kind,Asset | Export-Csv (Join-Path $Root 'docs\ART-ICON-SOURCES.csv') -NoTypeInformation -Encoding UTF8
Write-Output "Generated $($manifest.Count) explicit official identity icons."
