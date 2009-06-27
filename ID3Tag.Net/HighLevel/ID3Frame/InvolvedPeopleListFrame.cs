using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ID3Tag.HighLevel;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel.ID3Frame
{

    /// <summary>
    /// Since there might be a lot of people contributing to an audio file in various ways, 
    /// such as musicians and technicians, the 'Text information frames' are often insufficient 
    /// to list everyone involved in a project. 
    /// The 'Involved people list' is a frame containing the names of those involved, 
    /// and how they were involved. The body simply contains a terminated string with the 
    /// involvement directly followed by a terminated string with the involvee followed 
    /// by a new involvement and so on. There may only be one "IPLS" frame in each tag.
    /// </summary>
    //public class InvolvedPeopleListFrame : Frame
    //{
    //    /*
    //     * <Header for 'Involved people list', ID: "IPLS"> 
    //        Text encoding    $xx
    //        People list strings    <text strings according to encoding>
    //     * 
    //     *  aaaaaa0bbbbbb0  ( Involvement, Involvee )
    //     *  cccccc0ddddd0
    //     *  eee0fffffffffff0
    //     */

    ////   TODO: Add Validator for this frame!

    //    public override RawFrame Convert()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override void Import(RawFrame rawFrame)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override FrameType Type
    //    {
    //        get { return FrameType.InvolvedPeopleList; }
    //    }
    //}
}
