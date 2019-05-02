using System.Collections.Generic;
using UnityEngine;
using PatternUtils;

namespace EZR
{
    public static class Judgment
    {
        public static void Loop(List<NoteInLine>[] noteInLines, JudgmentAnimCTL judgmentAnim, FlareAnimCTL[] flarePlayList, FlareAnimCTL[] LongflarePlayList)
        {
            for (int i = 0; i < PlayManager.NumLines; i++)
            {
                if (noteInLines[i].Count > 0 && noteInLines[i][0] != null)
                {
                    var noteInLine = noteInLines[i][0];
                    if (noteInLine.IsLongPressed)
                    {
                        // 长音连击
                        var longNoteCombo = (int)((System.Math.Min(PlayManager.Position, noteInLine.Position + noteInLine.NoteLength) - noteInLine.Position) / 16);
                        if (longNoteCombo > noteInLine.LongNoteCombo)
                        {
                            var delta = longNoteCombo - noteInLine.LongNoteCombo;
                            for (int j = 0; j < delta; j++)
                            {
                                judgmentAnim.Play(noteInLine.LongNoteJudgment);
                                PlayManager.addCombo();
                                PlayManager.addScore(noteInLine.LongNoteJudgment);
                                flarePlayList[i].Play(noteInLine.LongNoteJudgment);
                            }
                            noteInLine.LongNoteCombo = longNoteCombo;
                        }
                        // 自动结尾
                        if (noteInLine.Position + noteInLine.NoteLength <= PlayManager.Position)
                        {
                            var delta = longNoteCombo - noteInLine.LongNoteCombo;
                            for (int j = 0; j < delta; j++)
                            {
                                judgmentAnim.Play(noteInLine.LongNoteJudgment);
                                PlayManager.addCombo();
                                PlayManager.addScore(noteInLine.LongNoteJudgment);
                                flarePlayList[i].Play(noteInLine.LongNoteJudgment);
                            }
                            LongflarePlayList[i].IsStop = true;
                            noteInLine.IsDestroy = true;
                            noteInLines[i].RemoveAt(0);
                            goto endif;
                        }
                        // 按住不会miss
                        // if (noteInLine.Position + noteInLine.NoteLength - Position < -JudgmentDelta.GOOD / 2f * JudgmentDelta.Scale)
                        // {
                        //     judgmentAnim.Play("miss");
                        //     comboBreak();
                        //     LongflarePlayList[i].IsStop = true;
                        //     noteInLines[i].RemoveAt(0);
                        // }
                    }

                    // FAIL
                    else if (noteInLine.NoteLength > 0)
                    {
                        // 长音将来可能会改不同判定
                        if (noteInLine.Position - PlayManager.Position < -JudgmentDelta.MISS / 2f * JudgmentDelta.Scale)
                        {
                            judgmentAnim.Play("fail");
                            PlayManager.comboBreak();
                            noteInLines[i].RemoveAt(0);
                            goto endif;
                        }
                    }
                    else
                    {
                        if (noteInLine.Position - PlayManager.Position < -JudgmentDelta.MISS / 2f * JudgmentDelta.Scale)
                        {
                            judgmentAnim.Play("fail");
                            PlayManager.comboBreak();
                            noteInLines[i].RemoveAt(0);
                            goto endif;
                        }
                    }
                }

            endif: if (noteInLines[i].Count > 0 && noteInLines[i][0] != null)
                {
                    // Auto play
                    var noteInLine = noteInLines[i][0];
                    if (PlayManager.IsAutoPlay)
                    {
                        if (noteInLine.NoteLength > 6)
                        {
                            // 长音
                            if (!noteInLine.IsLongPressed && noteInLine.Position <= PlayManager.Position)
                            {
                                PlayManager.addCombo();
                                PlayManager.addScore("kool");
                                judgmentAnim.Play("kool");
                                var note = PlayManager.TimeLines.Lines[i].Notes[noteInLine.index];
                                noteInLine.IsLongPressed = true;
                                noteInLine.LongNoteJudgment = "kool";
                                LongflarePlayList[i].Play();
                                flarePlayList[i].Play();
                                MemorySound.playSound(note.id, note.vol, note.pan, MemorySound.Main);
                            }
                            // else if (noteInLine.isLongPressed &&
                            // noteInLine.Position + noteInLine.NoteLength <= Position)
                            // {
                            //     noteInLine.isLongPressed = false;
                            //     noteInLine.isDestroy = true;
                            //     LongflarePlayList[i].isStop = true;
                            //     flarePlayList[i].Play();
                            //     noteInLines[i].RemoveAt(0);
                            // }
                        }
                        else
                        {
                            // 短音
                            if (noteInLine.Position <= PlayManager.Position)
                            {
                                PlayManager.addCombo();
                                PlayManager.addScore("kool");
                                judgmentAnim.Play("kool");
                                var note = PlayManager.TimeLines.Lines[i].Notes[noteInLine.index];
                                noteInLine.IsDestroy = true;
                                flarePlayList[i].Play();
                                MemorySound.playSound(note.id, note.vol, note.pan, MemorySound.Main);
                                noteInLines[i].RemoveAt(0);
                            }
                        }
                    }
                }
            }
        }

        public static void InputEvent(bool state, int keyId, List<NoteInLine>[] noteInLines, JudgmentAnimCTL judgmentAnim, FlareAnimCTL[] flarePlayList, FlareAnimCTL[] LongflarePlayList)
        {
            // 判定
            if (state)
            {
                Pattern.Note note;
                NoteInLine noteInLine;

                if (noteInLines[keyId].Count > 0 && noteInLines[keyId][0] != null)
                {
                    noteInLine = noteInLines[keyId][0];
                    note = PlayManager.TimeLines.Lines[keyId].Notes[noteInLine.index];

                    double judgmentDelta = System.Math.Abs(noteInLine.Position - PlayManager.Position);

                    if (note.length > 6)
                    {
                        if (judgmentDelta <= JudgmentDelta.KOOL / 2f * JudgmentDelta.Scale)
                        {
                            judgmentAnim.Play("kool");
                            flarePlayList[keyId].Play();
                            LongflarePlayList[keyId].Play();
                            PlayManager.addCombo();
                            PlayManager.addScore("kool");
                            noteInLine.IsLongPressed = true;
                            noteInLine.LongNoteJudgment = "kool";
                        }
                        else if (judgmentDelta > JudgmentDelta.KOOL / 2f * JudgmentDelta.Scale && judgmentDelta <= JudgmentDelta.COOL / 2f * JudgmentDelta.Scale)
                        {
                            judgmentAnim.Play("cool");
                            flarePlayList[keyId].Play();
                            LongflarePlayList[keyId].Play();
                            PlayManager.addCombo();
                            PlayManager.addScore("cool");
                            noteInLine.IsLongPressed = true;
                            noteInLine.LongNoteJudgment = "cool";
                        }
                        else if (judgmentDelta > JudgmentDelta.COOL / 2f * JudgmentDelta.Scale && judgmentDelta <= JudgmentDelta.GOOD / 2f * JudgmentDelta.Scale)
                        {
                            judgmentAnim.Play("good");

                            flarePlayList[keyId].Play("good");
                            LongflarePlayList[keyId].Play("good");
                            PlayManager.addCombo();
                            PlayManager.addScore("good");
                            noteInLine.IsLongPressed = true;
                            noteInLine.LongNoteJudgment = "good";
                        }
                        else if (judgmentDelta > JudgmentDelta.GOOD / 2f * JudgmentDelta.Scale && judgmentDelta <= JudgmentDelta.MISS / 2f * JudgmentDelta.Scale)
                        {
                            PlayManager.comboBreak();
                            judgmentAnim.Play("miss");
                            noteInLines[keyId].RemoveAt(0);
                        }
                    }
                    else
                    {
                        if (judgmentDelta <= JudgmentDelta.KOOL / 2f * JudgmentDelta.Scale)
                        {
                            judgmentAnim.Play("kool");
                            flarePlayList[keyId].Play();
                            PlayManager.addCombo();
                            PlayManager.addScore("kool");
                            noteInLine.IsDestroy = true;
                            noteInLines[keyId].RemoveAt(0);
                        }
                        else if (judgmentDelta > JudgmentDelta.KOOL / 2f * JudgmentDelta.Scale && judgmentDelta <= JudgmentDelta.COOL / 2f * JudgmentDelta.Scale)
                        {
                            judgmentAnim.Play("cool");
                            flarePlayList[keyId].Play();
                            PlayManager.addCombo();
                            PlayManager.addScore("cool");
                            noteInLine.IsDestroy = true;
                            noteInLines[keyId].RemoveAt(0);
                        }
                        else if (judgmentDelta > JudgmentDelta.COOL / 2f * JudgmentDelta.Scale && judgmentDelta <= JudgmentDelta.GOOD / 2f * JudgmentDelta.Scale)
                        {
                            judgmentAnim.Play("good");
                            flarePlayList[keyId].Play("good");
                            PlayManager.addCombo();
                            PlayManager.addScore("good");
                            noteInLine.IsDestroy = true;
                            noteInLines[keyId].RemoveAt(0);
                        }
                        else if (judgmentDelta > JudgmentDelta.GOOD / 2f * JudgmentDelta.Scale && judgmentDelta <= JudgmentDelta.MISS / 2f * JudgmentDelta.Scale)
                        {
                            PlayManager.comboBreak();
                            judgmentAnim.Play("miss");
                            noteInLines[keyId].RemoveAt(0);
                        }
                    }
                }
                else
                {
                    noteInLine = null;
                    note = PlayManager.TimeLines.Lines[keyId].Notes[Mathf.Min(PlayManager.TimeLines.LinesIndex[keyId], PlayManager.TimeLines.Lines[keyId].Notes.Count - 1)];
                }

                var channel = MemorySound.playSound(note.id, note.vol, note.pan, MemorySound.Main);
                if (noteInLine != null && note.length > 6)
                {
                    noteInLine.NoteSound = channel;
                }
            }
            else if (noteInLines[keyId].Count > 0 && noteInLines[keyId][0] != null)
            {
                var noteInLine = noteInLines[keyId][0];
                if (noteInLine.IsLongPressed)
                {
                    noteInLine.IsLongPressed = false;
                    LongflarePlayList[keyId].IsStop = true;

                    bool needStopSound = false;
                    double judgmentDelta = System.Math.Abs(noteInLine.Position + noteInLine.NoteLength - PlayManager.Position);
                    if (judgmentDelta > JudgmentDelta.COOL / 2f * JudgmentDelta.Scale && judgmentDelta <= JudgmentDelta.GOOD / 2f * JudgmentDelta.Scale)
                    {
                        judgmentAnim.Play("good");
                        PlayManager.addCombo();
                        PlayManager.addScore("good");
                        flarePlayList[keyId].Play("good");
                        needStopSound = true;
                    }
                    else if (judgmentDelta > JudgmentDelta.GOOD / 2f * JudgmentDelta.Scale)
                    {
                        PlayManager.comboBreak();
                        judgmentAnim.Play("miss");
                        needStopSound = true;
                    }

                    if (needStopSound && noteInLine.NoteSound != null)
                    {
                        ((FMOD.Channel)noteInLine.NoteSound).stop();
                    }
                    noteInLines[keyId].RemoveAt(0);
                }
            }
        }
    }
}