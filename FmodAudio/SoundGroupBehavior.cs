namespace FmodAudio
{
    public enum SoundGroupBehavior : int
    {
        Fail,              /* Any sound played that puts the sound count over the SoundGroup::setMaxAudible setting, will simply fail during System::playSound. */
        Mute,              /* Any sound played that puts the sound count over the SoundGroup::setMaxAudible setting, will be silent, then if another sound in the group stops the sound that was silent before becomes audible again. */
        StealLowest,       /* Any sound played that puts the sound count over the SoundGroup::setMaxAudible setting, will steal the quietest / least important sound playing in the group. */

        MAX,               /* Maximum number of sound group behaviors. */
    }
}
