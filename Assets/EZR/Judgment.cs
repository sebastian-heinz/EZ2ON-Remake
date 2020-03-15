using UnityEngine;
using PatternUtils;

namespace EZR
{
    public static class Judgment
    {
        public static int LongNoteComboStep => 12;

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
                        int longNoteCombo = Mathf.Min(noteInLine.LongNoteCount, (int)(
                            (System.Math.Min(PlayManager.Position, noteInLine.Position + noteInLine.NoteLength) - noteInLine.Position) /
                            (LongNoteComboStep * JudgmentDelta.MeasureScale)) + 1);
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

                // Auto play
                if (PlayManager.IsAutoPlay)
                {
                    if (noteInLines[i].Count > 0 && noteInLines[i].Peek() != null)
                    {
                        var noteInLine = noteInLines[i].Peek();
                        if (noteInLine.NoteLength > 6)
                        {
                            // 长音
                            if (!noteInLine.IsLongPressed && noteInLine.Position <= PlayManager.Position)
                            {
                                var note = PlayManager.TimeLine.Lines[i].Notes[noteInLine.Index];
                                noteInLine.IsLongPressed = true;
                                noteInLine.LongNoteJudgment = JudgmentType.Kool;
                                LongflarePlayList[i].Play();
                                MemorySound.PlaySound(note.id, note.volume, note.pan, MemorySound.Main);
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
                                var note = PlayManager.TimeLine.Lines[i].Notes[noteInLine.Index];
                                noteInLine.IsDestroy = true;
                                flarePlayList[i].Play();
                                MemorySound.PlaySound(note.id, note.volume, note.pan, MemorySound.Main);
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
                    note = PlayManager.TimeLine.Lines[keyId].Notes[noteInLine.Index];

                    double judgmentDelta = noteInLine.Position - PlayManager.Position;
                    bool isFast = judgmentDelta > 0;
                    judgmentDelta = System.Math.Abs(judgmentDelta);

                    // 长音
                    if (note.length > 6)
                    {
                        if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Kool, 1))
                        {
                            LongflarePlayList[keyId].Play();
                            noteInLine.IsLongPressed = true;
                            noteInLine.LongNoteJudgment = JudgmentType.Kool;
                        }
                        else if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Cool, 1))
                        {
                            judgmentAnim.ShowFastSlow(isFast);
                            LongflarePlayList[keyId].Play();
                            noteInLine.IsLongPressed = true;
                            noteInLine.LongNoteJudgment = JudgmentType.Cool;
                        }
                        else if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Good, 1))
                        {
                            judgmentAnim.ShowFastSlow(isFast);
                            LongflarePlayList[keyId].Play(JudgmentType.Good);
                            noteInLine.IsLongPressed = true;
                            noteInLine.LongNoteJudgment = JudgmentType.Good;
                        }
                        else if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Miss, 1))
                        {
                            PlayManager.ComboBreak();
                            PlayManager.AddScore(JudgmentType.Miss);
                            judgmentAnim.Play(JudgmentType.Miss);
                            judgmentAnim.ShowFastSlow(isFast);
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
                            judgmentAnim.ShowFastSlow(isFast);
                            flarePlayList[keyId].Play();
                            noteInLine.IsDestroy = true;
                            noteInLines[keyId].Dequeue();
                        }
                        else if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Good, 0.5f))
                        {
                            PlayManager.AddCombo();
                            PlayManager.AddScore(JudgmentType.Good);
                            judgmentAnim.Play(JudgmentType.Good);
                            judgmentAnim.ShowFastSlow(isFast);
                            flarePlayList[keyId].Play(JudgmentType.Good);
                            noteInLine.IsDestroy = true;
                            noteInLines[keyId].Dequeue();
                        }
                        else if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Miss, 0.5f))
                        {
                            PlayManager.ComboBreak();
                            PlayManager.AddScore(JudgmentType.Miss);
                            judgmentAnim.Play(JudgmentType.Miss);
                            judgmentAnim.ShowFastSlow(isFast);
                            noteInLines[keyId].Dequeue();
                        }
                    }
                }
                else
                {
                    noteInLine = null;
                    if (PlayManager.TimeLine.Lines[keyId].Notes.Count > 0)
                        note = PlayManager.TimeLine.Lines[keyId].Notes[Mathf.Min(PlayManager.TimeLine.LinesIndex[keyId], PlayManager.TimeLine.Lines[keyId].Notes.Count - 1)];
                    else
                        note = null;
                }

                FMOD.Channel? channel = null;
                if (note != null)
                    channel = MemorySound.PlaySound(note.id, note.volume, note.pan, MemorySound.Main);
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
                    double judgmentDelta = noteInLine.Position + noteInLine.NoteLength - PlayManager.Position;
                    bool isFast = judgmentDelta > 0;
                    judgmentDelta = System.Math.Abs(judgmentDelta);
                    if (judgmentDelta <= JudgmentDelta.GetJudgmentDelta(JudgmentType.Cool, 1))
                    {
                        var delta = noteInLine.LongNoteCount - noteInLine.LongNoteCombo;
                        for (int j = 0; j < delta; j++)
                        {
                            PlayManager.AddCombo();
                            PlayManager.AddScore(noteInLine.LongNoteJudgment);
                            judgmentAnim.Play(noteInLine.LongNoteJudgment);
                            flarePlayList[keyId].Play(noteInLine.LongNoteJudgment);
                        }
                        if (!JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Kool, 1))
                            judgmentAnim.ShowFastSlow(isFast);
                        noteInLine.IsDestroy = true;
                    }
                    else if (JudgmentDelta.CompareJudgmentDelta(judgmentDelta, JudgmentType.Good, 1))
                    {
                        var delta = noteInLine.LongNoteCount - noteInLine.LongNoteCombo;
                        for (int j = 0; j < delta; j++)
                        {
                            PlayManager.AddCombo();
                            PlayManager.AddScore(JudgmentType.Good);
                            judgmentAnim.Play(JudgmentType.Good);
                            flarePlayList[keyId].Play(JudgmentType.Good);
                        }
                        judgmentAnim.ShowFastSlow(isFast);
                        needStopSound = true;
                    }
                    else if (judgmentDelta > JudgmentDelta.GetJudgmentDelta(JudgmentType.Good, 1))
                    {
                        PlayManager.ComboBreak();
                        PlayManager.AddScore(JudgmentType.Miss);
                        judgmentAnim.Play(JudgmentType.Miss);
                        judgmentAnim.ShowFastSlow(isFast);
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