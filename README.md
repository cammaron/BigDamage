
# BigDamage - Erenshor mod

## Compatibility

Created for Erenshor v0.2 (early access).

## Installation

This mod requires [BepInEx](https://docs.bepinex.dev/articles/user_guide/installation/index.html) which is a simple and light weight platform to support mods for Unity games, so install that in Erenshor's directory first if you haven't already! For a Steam installation, the path should look like "\Steam\steamapps\common\Erenshor\BepInEx"

Grab the [latest release of BigDamage](https://github.com/cammaron/BigDamage/releases) and place it in the BepInEx/plugins folder (if it doesn't exist yet, feel free to create it)

## Overview

In vanilla Erenshor, I've found the floating damage numbers extremely difficult to read. That's partly because they have a colour gradient, and partly because they can be very small. That's because they're created in world space in the game at an absolute size, meaning that if you damage an enemy further away from you, the damage numbers are smaller -- likewise if you're simply more zoomed out. Additionally, by default, floating damage numbers only show for damage dealt within a quite short range of your character, so for example an Arcanist doing a spell at long range wouldn't see the damage pop up at all.

BigDamage does the following:

- Automatically scale floating damage numbers based on distance from camera, so they won't ever be too small to read (and are generally fairly consistent)
- Makes damage numbers larger in the first place
- Removes the gradient, instead opting for completely solid bold colours representin different damage types
- Increases the range at which numbers will be shown
- Fixes an issue that prevented floating damage numbers (and damage source attribution) being applied to damage ticks from DoT spells

The mod has a number of configurable properties if you want to adjust it further, which, as standard for BepInEx mods, will appear in a generated .cfg file at BepInEx/config the first time the mod is run with the game.

maxDistance -- The distance a damage popup can be at before being hidden from the player. Higher = view hits further away.
baseSizeFactor -- An initial multiplier to damage popup font size. Just makin' it bigger regardless.
distanceToDoubleSize -- Every X units of distance, the damage popup text scale is doubled. Scaling over distance is the main purpose of this mod, as damage occurring further away needs to be larger in world space to be visible.
enableColourChange -- Attempts to increase damage readability by removing the colour gradient and choosing simple, bold colours. Set this to false if you prefer the original style.

As well as options to change the damage colours for each element.