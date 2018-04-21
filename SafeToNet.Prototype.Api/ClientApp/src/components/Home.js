import React, { Component } from 'react';
import { Button } from 'react-bootstrap';
import DetectRTC from 'detectrtc';
import RecordRTC from 'recordrtc';

const RECORDER_STOP_DELAY_MS = 500;

export class Home extends Component {
  constructor() {
    super();
    this.state = { recorder: null, audioStream: null, errorMsg: null };

    this.initAudio();
  }

  initAudio() {
    if (DetectRTC.isWebRTCSupported === false) {
      this.state.errorMsg = 'Browser not supported. Please use the latest Chrome/Firefox/Edge on Windows or Android';
    }

    navigator.mediaDevices.getUserMedia({ audio: true }).then(function (stream) {
      console.log('Media stream created.');
      this.state.audioStream = stream;
    }).catch(function (e) {
        console.log('No live audio input: ' + e);
    });
  }

  tapStart(e, touch) {
    var options = {
        //recorderType: StereoAudioRecorder,
        mimeType: 'audio/wav',
        numberOfAudioChannels: 1,
        desiredSampRate: 16000
    };

    if (!this.state.recorder) {
        this.state.recorder = new RecordRTC(this.state.audioStream, options);
        this.state.recorder.initRecorder();
    }
    
    console.log('Recorder initialised.');

    console.log("Start recording!");

    this.state.recorder && this.state.recorder.startRecording();
  }

  tapEnd(e, touch) {
    setTimeout(function () {
        this.state.recorder && this.state.recorder.stopRecording(this.stoppedRecording);
    }, RECORDER_STOP_DELAY_MS);
  }

  stoppedRecording() {
    console.log("Stopped recording.");
    var blob = this.state.recorder.getBlob();
    this.state.recorder.initRecorder();
  }

  displayName = Home.name

  render() {
    return (
      <div>
        <h1>Recipe Search 3000™</h1>

        <Button bsStyle="primary" bsSize="large">Search</Button>

      </div>
    );
  }
}
