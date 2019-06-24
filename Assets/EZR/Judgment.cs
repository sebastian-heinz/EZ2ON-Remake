using UnityEngine;
using PatternUtils;

namespace EZR
{
    public static class Judgment
    {
        public static float LongNoteComboStep { get => 12 * JudgmentDelta.MeasureScale; }

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
                        int longNoteCombo = (int)((System.Math.Min(PlayManager.Position, noteInLine.Position + noteInLine.NoteLength) - noteInLine.Position) / LongNoteComboStep);
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
                            LongflarePlayList[i].IsStop = true;
                            noteInLine.IsDestroy = true;
                            noteInLines[i].Dequeue();
                        }
                    }

                    // FAIL
                    else if (noteInLine.NoteLength > 6)
                    {
                        // 长音
                        if (noteInLine.Position - PlayManager.Position < -JudgmentDelta.GetJudgmentDelta(JudgmentType.Miss, 1))
                        {
                            PlayManager.ComboBreak();
                            PlayManager.AddScore(JudgmentType.Fail);
                            judgmentAnim.Play(JudgmentType.Fail);
                            noteInLines[i].Dequeue();
                        }
                    }
                    else
                    {
                        // 短音
                        if (noteInLine.Position - PlayManager.Position < -JudgmentDelta.GetJudgmentDelta(JudgmentType.Miss, 0.5f))
                        {
                            PlayManager.ComboBreak();
                            PlayManager.AddScore(JudgmentType.Fail);
                            judgmentAnim.Play(JudgmentType.Fail);
                            noteInLines[i].Dequeue();
                        }
                    }
                }

                if (noteInLines[i].Count > 0 && noteInLines[i].Peek() != null)
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
                                var note = PlayManager.TimeLines.Lines[i].Notes[noteInLine.Index];
                                noteInLine.IsLongPressed = true;
                                noteInLine.LongNoteJudgment = JudgmentType.Kool;
                                LongflarePlayList[i].Play();
                                flarePlayList[i].Play();
                                MemorySound.PlaySound(note.id, note.vol, note.pan, MemorySound.Main);
                            }
                        }
                        else
                        {
                            // 短音
                            if (noteInLine.Position <= PlayManager.Position)
                            {
                                PlayManager.AddCombo();
                                PlayManager.AddScore(JudgmentType.Kool);
                                judgmentAnim.Play(JudgmentType.Kool);
                                var note = PlayManager.TimeLines.Lines[i].Notes[noteInLine.Index];
                                noteInLine.IsDestroy = true;
                                flarePlayList[i].Play();
                                MemorySound.PlaySound(note.id, note.vol, note.pan, MemorySound.Main);
                                noteInLines[i].Dequeue();
                            }
                        }
                    }
                }
            }
        }

        public static void InputEvent(bool state, int keyId, Queue<NoteInLine>[] noteInLines, JudgmentAnimCTL judgmentAnim, FlareAnimCTL[] flarePlayList, FlareAnimCTL[] LongflarePlayList)
        {
            // 按下
            if (state)
            {
                Pattern.Note note;
                NoteInLine noteInLine;

                if (noteInLines[keyId].Count > 0 && noteInLines[keyId].Peek() != null)
                {
                    noteInLine = noteInLines[keyId].Peek();
                    note = PlayManager.TimeLines.Lines[keyId].Notes[noteInLine.Index];

                    double judgmentDelta = System.Math.Abs(noteInLine.Position - PlayManager.Position);

                    // 长音
                    if (note.length > 6)
                    {
                        if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Kool, 1))
                        {
                            PlayManager.AddCombo();
                            PlayManager.AddScore(JudgmentType.Kool);
                            judgmentAnim.Play(JudgmentType.Kool);
                            flarePlayList[keyId].Play();
                            LongflarePlayList[keyId].Play();
                            noteInLine.IsLongPressed = true;
                            noteInLine.LongNoteJudgment = JudgmentType.Kool;
                        }
                        else if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Cool, 1))
                        {
                            PlayManager.AddCombo();
                            PlayManager.AddScore(JudgmentType.Cool);
                            judgmentAnim.Play(JudgmentType.Cool);
                            flarePlayList[keyId].Play();
                            LongflarePlayList[keyId].Play();
                            noteInLine.IsLongPressed = true;
                            noteInLine.LongNoteJudgment = JudgmentType.Cool;
                        }
                        else if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Good, 1))
                        {
                            PlayManager.AddCombo();
                            PlayManager.AddScore(JudgmentType.Good);
                            judgmentAnim.Play(JudgmentType.Good);
                            flarePlayList[keyId].Play(JudgmentType.Good);
                            LongflarePlayList[keyId].Play(JudgmentType.Good);
                            noteInLine.IsLongPressed = true;
                            noteInLine.LongNoteJudgment = JudgmentType.Good;
                        }
                        else if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Miss, 1))
                        {
                            PlayManager.ComboBreak();
                            PlayManager.AddScore(JudgmentType.Miss);
                            judgmentAnim.Play(JudgmentType.Miss);
                            noteInLines[keyId].Dequeue();
                        }
                    }
                    // 短音
                    else
                    {
                        if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Kool, 0.5f))
                        {
                            PlayManager.AddCombo();
                            PlayManager.AddScore(JudgmentType.Kool);
                            judgmentAnim.Play(JudgmentType.Kool);
                            flarePlayList[keyId].Play();
                            noteInLine.IsDestroy = true;
                            noteInLines[keyId].Dequeue();
                        }
                        else if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Cool, 0.5f))
                        {
                            PlayManager.AddCombo();
                            PlayManager.AddScore(JudgmentType.Cool);
                            judgmentAnim.Play(JudgmentType.Cool);
                            flarePlayList[keyId].Play();
                            noteInLine.IsDestroy = true;
                            noteInLines[keyId].Dequeue();
                        }
                        else if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Good, 0.5f))
                        {
                            PlayManager.AddCombo();
                            PlayManager.AddScore(JudgmentType.Good);
                            judgmentAnim.Play(JudgmentType.Good);
                            flarePlayList[keyId].Play(JudgmentType.Good);
                            noteInLine.IsDestroy = true;
                            noteInLines[keyId].Dequeue();
                        }
                        else if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Miss, 0.5f))
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
                    if (PlayManager.TimeLines.Lines[keyId].Notes.Count > 0)
                        note = PlayManager.TimeLines.Lines[keyId].Notes[Mathf.Min(PlayManager.TimeLines.LinesIndex[keyId], PlayManager.TimeLines.Lines[keyId].Notes.Count - 1)];
                    else
                        note = null;
                }

                FMOD.Channel? channel = null;
                if (note != null)
                    channel = MemorySound.PlaySound(note.id, note.vol, note.pan, MemorySound.Main);
                if (noteInLine != null && note.length > 6)
                {
                    noteInLine.NoteSound = channel;
                }
            }
            // 松开
            else if (noteInLines[keyId].Count > 0 && noteInLines[keyId].Peek() != null)
            {
                var noteInLine = noteInLines[keyId].Peek();
                if (noteInLine.IsLongPressed)
                {
                    noteInLine.IsLongPressed = false;
                    LongflarePlayList[keyId].IsStop = true;

                    bool needStopSound = false;
                    double judgmentDelta = System.Math.Abs(noteInLine.Position + noteInLine.NoteLength - PlayManager.Position);
                    int longNoteCombo = (int)(noteInLine.NoteLength / LongNoteComboStep);
                    if (judgmentDelta <= JudgmentDelta.GetJudgmentDelta(JudgmentType.Cool, 0.5f))
                    {
                        var delta = longNoteCombo - noteInLine.LongNoteCombo;
                        for (int j = 0; j < delta; j++)
                        {
                            PlayManager.AddCombo();
                            PlayManager.AddScore(noteInLine.LongNoteJudgment);
                            judgmentAnim.Play(noteInLine.LongNoteJudgment);
                            flarePlayList[keyId].Play(noteInLine.LongNoteJudgment);
                        }
                        noteInLine.IsDestroy = true;
                    }
                    else if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Good, 0.5f))
                    {
                        var delta = longNoteCombo - noteInLine.LongNoteCombo;
                        for (int j = 0; j < delta; j++)
                        {
                            PlayManager.AddCombo();
                            PlayManager.AddScore(JudgmentType.Good);
                            judgmentAnim.Play(JudgmentType.Good);
                            flarePlayList[keyId].Play(JudgmentType.Good);
                        }
                        needStopSound = true;
                    }
                    else if (judgmentDelta > JudgmentDelta.GetJudgmentDelta(JudgmentType.Good, 0.5f))
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