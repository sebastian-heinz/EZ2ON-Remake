using System.Collections.Generic;
using UnityEngine;
using PatternUtils;

namespace EZR
{
    public static class Judgment
    {
        static int longNoteComboStep = 12;

        public static void Loop(Queue<NoteInLine>[] noteInLines, JudgmentAnimCTL judgmentAnim, FlareAnimCTL[] flarePlayList, FlareAnimCTL[] LongflarePlayList)
        {
            for (int i = 0; i < PlayManager.NumLines; i++)
            {
                if (noteInLines[i].Count > 0 && noteInLines[i].Peek() != null)
                {
                    var noteInLine = noteInLines[i].Peek();
                    if (noteInLine.IsLongPressed)
                    {
                        // 长音连击
                        var longNoteCombo = (int)((System.Math.Min(PlayManager.Position, noteInLine.Position + noteInLine.NoteLength) - noteInLine.Position) / longNoteComboStep);
                        if (longNoteCombo > noteInLine.LongNoteCombo)
                        {
                            var delta = longNoteCombo - noteInLine.LongNoteCombo;
                            for (int j = 0; j < delta; j++)
                            {
                                PlayManager.AddCombo();
                                PlayManager.AddScore(noteInLine.LongNoteJudgment);
                                judgmentAnim.Play(noteInLine.LongNoteJudgment);
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
                                PlayManager.AddCombo();
                                PlayManager.AddScore(noteInLine.LongNoteJudgment);
                                judgmentAnim.Play(noteInLine.LongNoteJudgment);
                                flarePlayList[i].Play(noteInLine.LongNoteJudgment);
                            }
                            LongflarePlayList[i].IsStop = true;
                            noteInLine.IsDestroy = true;
                            noteInLines[i].Dequeue();
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
                        if (noteInLine.Position - PlayManager.Position < -JudgmentDelta.GetJudgmentDelta(JudgmentType.Miss))
                        {
                            PlayManager.ComboBreak();
                            PlayManager.AddScore(JudgmentType.Fail);
                            judgmentAnim.Play(JudgmentType.Fail);
                            noteInLines[i].Dequeue();
                            goto endif;
                        }
                    }
                    else
                    {
                        if (noteInLine.Position - PlayManager.Position < -JudgmentDelta.GetJudgmentDelta(JudgmentType.Miss))
                        {
                            PlayManager.ComboBreak();
                            PlayManager.AddScore(JudgmentType.Fail);
                            judgmentAnim.Play(JudgmentType.Fail);
                            noteInLines[i].Dequeue();
                            goto endif;
                        }
                    }
                }

            endif: if (noteInLines[i].Count > 0 && noteInLines[i].Peek() != null)
                {
                    // Auto play
                    var noteInLine = noteInLines[i].Peek();
                    if (PlayManager.IsAutoPlay)
                    {
                        if (noteInLine.NoteLength > 6)
                        {
                            // 长音
                            if (!noteInLine.IsLongPressed && noteInLine.Position <= PlayManager.Position)
                            {
                                PlayManager.AddCombo();
                                PlayManager.AddScore(JudgmentType.Kool);
                                judgmentAnim.Play(JudgmentType.Kool);
                                var note = PlayManager.TimeLines.Lines[i].Notes[noteInLine.index];
                                noteInLine.IsLongPressed = true;
                                noteInLine.LongNoteJudgment = JudgmentType.Kool;
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
                                PlayManager.AddCombo();
                                PlayManager.AddScore(JudgmentType.Kool);
                                judgmentAnim.Play(JudgmentType.Kool);
                                var note = PlayManager.TimeLines.Lines[i].Notes[noteInLine.index];
                                noteInLine.IsDestroy = true;
                                flarePlayList[i].Play();
                                MemorySound.playSound(note.id, note.vol, note.pan, MemorySound.Main);
                                noteInLines[i].Dequeue();
                            }
                        }
                    }
                }
            }
        }

        public static void InputEvent(bool state, int keyId, Queue<NoteInLine>[] noteInLines, JudgmentAnimCTL judgmentAnim, FlareAnimCTL[] flarePlayList, FlareAnimCTL[] LongflarePlayList)
        {
            // 判定
            if (state)
            {
                Pattern.Note note;
                NoteInLine noteInLine;

                if (noteInLines[keyId].Count > 0 && noteInLines[keyId].Peek() != null)
                {
                    noteInLine = noteInLines[keyId].Peek();
                    note = PlayManager.TimeLines.Lines[keyId].Notes[noteInLine.index];

                    double judgmentDelta = System.Math.Abs(noteInLine.Position - PlayManager.Position);

                    if (note.length > 6)
                    {
                        if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Kool))
                        {
                            PlayManager.AddCombo();
                            PlayManager.AddScore(JudgmentType.Kool);
                            judgmentAnim.Play(JudgmentType.Kool);
                            flarePlayList[keyId].Play();
                            LongflarePlayList[keyId].Play();
                            noteInLine.IsLongPressed = true;
                            noteInLine.LongNoteJudgment = JudgmentType.Kool;
                        }
                        else if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Cool))
                        {
                            PlayManager.AddCombo();
                            PlayManager.AddScore(JudgmentType.Cool);
                            judgmentAnim.Play(JudgmentType.Cool);
                            flarePlayList[keyId].Play();
                            LongflarePlayList[keyId].Play();
                            noteInLine.IsLongPressed = true;
                            noteInLine.LongNoteJudgment = JudgmentType.Cool;
                        }
                        else if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Good))
                        {
                            PlayManager.AddCombo();
                            PlayManager.AddScore(JudgmentType.Good);
                            judgmentAnim.Play(JudgmentType.Good);
                            flarePlayList[keyId].Play(JudgmentType.Good);
                            LongflarePlayList[keyId].Play(JudgmentType.Good);
                            noteInLine.IsLongPressed = true;
                            noteInLine.LongNoteJudgment = JudgmentType.Good;
                        }
                        else if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Miss))
                        {
                            PlayManager.ComboBreak();
                            PlayManager.AddScore(JudgmentType.Miss);
                            judgmentAnim.Play(JudgmentType.Miss);
                            noteInLines[keyId].Dequeue();
                        }
                    }
                    else
                    {
                        if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Kool))
                        {
                            PlayManager.AddCombo();
                            PlayManager.AddScore(JudgmentType.Kool);
                            judgmentAnim.Play(JudgmentType.Kool);
                            flarePlayList[keyId].Play();
                            noteInLine.IsDestroy = true;
                            noteInLines[keyId].Dequeue();
                        }
                        else if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Cool))
                        {
                            PlayManager.AddCombo();
                            PlayManager.AddScore(JudgmentType.Cool);
                            judgmentAnim.Play(JudgmentType.Cool);
                            flarePlayList[keyId].Play();
                            noteInLine.IsDestroy = true;
                            noteInLines[keyId].Dequeue();
                        }
                        else if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Good))
                        {
                            PlayManager.AddCombo();
                            PlayManager.AddScore(JudgmentType.Good);
                            judgmentAnim.Play(JudgmentType.Good);
                            flarePlayList[keyId].Play(JudgmentType.Good);
                            noteInLine.IsDestroy = true;
                            noteInLines[keyId].Dequeue();
                        }
                        else if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Miss))
                        {
                            PlayManager.ComboBreak();
                            PlayManager.AddScore(JudgmentType.Miss);
                            judgmentAnim.Play(JudgmentType.Miss);
                            noteInLines[keyId].Dequeue();
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
            else if (noteInLines[keyId].Count > 0 && noteInLines[keyId].Peek() != null)
            {
                var noteInLine = noteInLines[keyId].Peek();
                if (noteInLine.IsLongPressed)
                {
                    noteInLine.IsLongPressed = false;
                    LongflarePlayList[keyId].IsStop = true;

                    bool needStopSound = false;
                    double judgmentDelta = System.Math.Abs(noteInLine.Position + noteInLine.NoteLength - PlayManager.Position);
                    if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Good))
                    {
                        PlayManager.AddCombo();
                        PlayManager.AddScore(JudgmentType.Good);
                        judgmentAnim.Play(JudgmentType.Good);
                        flarePlayList[keyId].Play(JudgmentType.Good);
                        needStopSound = true;
                    }
                    else if (judgmentDelta > JudgmentDelta.GetJudgmentDelta(JudgmentType.Good))
                    {
                        PlayManager.ComboBreak();
                        PlayManager.AddScore(JudgmentType.Miss);
                        judgmentAnim.Play(JudgmentType.Miss);
                        needStopSound = true;
                    }

                    if (needStopSound && noteInLine.NoteSound != null)
                    {
                        ((FMOD.Channel)noteInLine.NoteSound).stop();
                    }
                    noteInLines[keyId].Dequeue();
                }
            }
        }
    }
}