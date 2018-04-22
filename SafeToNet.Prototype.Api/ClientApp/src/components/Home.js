import React, { Component } from 'react';
import { Button, FormGroup, ControlLabel, FormControl } from 'react-bootstrap';
import DetectRTC from 'detectrtc';
import RecordRTC, { StereoAudioRecorder } from 'recordrtc';
import { RecipeList } from './RecipeList';

const API_URL = process.env.API_URL || "https://a51c7752.ngrok.io/";

let globalBlob;

export class Home extends Component {
  constructor() {
    super();
    this.state = { 
      errorMsg: null, 
      recorder: null, 
      audioBlob: null,
      text: '',
      loading: false,
      apiData: null,
      waiting: true,
      audioEnabled: true,
    };

    this.tapStart = this.tapStart.bind(this);
    this.stopRecord = this.stopRecord.bind(this);

    this.handleClick = this.handleClick.bind(this);
    this.handleChange = this.handleChange.bind(this);
    this.handleAudio = this.handleAudio.bind(this);

    if (DetectRTC.isWebRTCSupported === false) {
      this.setState({errorMsg: 'Browser not supported. Please use the latest Chrome/Firefox/Edge on Windows or Android'});
      this.setState({audioEnabled: false});
    }

    navigator.mediaDevices.getUserMedia({audio: true}).then(stream => {
      var options = {
          recorderType: StereoAudioRecorder,
          mimeType: 'audio/wav',
          numberOfAudioChannels: 1,
          desiredSampRate: 16000
      };

      var recorder = new RecordRTC(stream, options);
      this.setState({recorder: recorder});
    }).catch(error => { alert(error); });

    setTimeout(() => { 
      this.handleClick();
    }, 100);
  }


  tapStart() {
    const { recorder } = this.state;

    this.setState({audioEnabled: false});

    recorder.initRecorder();

    console.log('Recorder initialised.');

    console.log("Start recording!");

    recorder.startRecording();
    
    setTimeout(() => {
      this.stopRecord();
    }, 4000);
  }

  stopRecord() {
    this.state.recorder.stopRecording(function() {
      console.log('stpRecordingGlobal');
      console.log(this);
      let blob = this.getBlob();
      console.log(blob);
      globalBlob = blob;
    });

    this.handleAudio(globalBlob);
  }

  handleAudio(blob) {
    console.log(blob);

    let formData = new FormData();
    formData.append('file', blob);

    fetch(API_URL + 'api/Search', {
      method: 'POST',
      body: formData,
      mode: 'no-cors'
    })
      .catch(error => console.error(error))
      .then(response => response.json())
      .catch(error => console.error(error))
      .then(data => {
          console.log(data);
          this.setState({ loading: false, apiData: data });
      });
    
    this.setState({audioEnabled: true});
  }

  handleChange(e) {
    this.setState({ text: e.target.value });

    if (!this.state.waiting) {
      setTimeout(() => {
        this.setState({waiting: true});
        this.handleClick();
      }, 1000);
    }
  }

  handleClick() {
    const { text } = this.state;
    console.log(text);
    this.setState({loading: true, waiting: false});

    if (text === '') {
      this.setState({loading: false});
      return;
    }

    fetch(API_URL + 'api/Search?query=' + text)
      .catch(error => console.error(error))
      .then(response => response.json())
      .catch(error => console.error(error))
      .then(data => {
          console.log(data);
          this.setState({ loading: false, apiData: data });
      });
  }

  displayName = Home.name

  render() {
    return (
      <div>
        <h1>Recipe Search</h1>
        <p>{this.state.errorMsg}</p>

         <Button bsStyle="primary" bsSize="large" disabled={!this.state.audioEnabled} onClick={this.tapStart}>Voice Search</Button>
        

          <FormGroup controlId="formBasicText">
            <ControlLabel>Write a search query</ControlLabel>
            <FormControl type="input" bsSize="large" value={this.state.text} onChange={this.handleChange} placeholder="enter some ingredients and/or ask for popular recipes" />
            {/* <Button bsStyle="primary" type="submit" bsSize="large" onClick={this.handleClick}>Search</Button> */}
          </FormGroup>

        
        
        <RecipeList apiData={this.state.apiData} loading={this.state.loading} />

      </div>
    );
  }
}
