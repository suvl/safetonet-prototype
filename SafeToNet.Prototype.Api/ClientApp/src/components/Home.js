import React, { Component } from 'react';
import { Button, FormGroup, ControlLabel, FormControl, HelpBlock } from 'react-bootstrap';
import DetectRTC from 'detectrtc';
import RecordRTC from 'recordrtc';
import { RecipeList } from './RecipeList';

const RECORDER_STOP_DELAY_MS = 500;

var globalBlob;

function stopRecordingGlobal() {
  globalBlob = this.getBlob();
}

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
      waiting: true
    };

    this.initAudio = this.initAudio.bind(this);
    this.tapStart = this.tapStart.bind(this);
    this.stopRecord = this.stopRecord.bind(this);

    this.handleClick = this.handleClick.bind(this);
    this.handleChange = this.handleChange.bind(this);

    this.initAudio();
    setTimeout(() => { 
      this.handleClick();
    }, 100);
  }

  initAudio() {
    if (DetectRTC.isWebRTCSupported === false) {
      this.setState({errorMsg: 'Browser not supported. Please use the latest Chrome/Firefox/Edge on Windows or Android'});
    }

    navigator.mediaDevices.getUserMedia({audio: true}).then(stream => {
      var options = {
          //recorderType: StereoAudioRecorder,
          mimeType: 'audio/wav',
          numberOfAudioChannels: 1,
          desiredSampRate: 16000
      };
      var recorder = new RecordRTC(stream, options);
      this.setState({recorder: recorder});
    });
  }

  tapStart() {
    const { recorder } = this.state;

    recorder.initRecorder();

    var component = this;
    
    console.log('Recorder initialised.');

    console.log("Start recording!");

    recorder.startRecording();
    
    setTimeout(() => {
      this.stopRecord();
    }, 5000);
  }

  stopRecord() {
    this.state.recorder.stopRecording(stopRecordingGlobal);

    console.log(globalBlob);
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

    if (text == '') {
      this.setState({loading: false});
      return;
    }

    fetch('http://localhost:5000/api/Search?query=' + text)
            .catch(error => console.error(error))
            .then(response => response.json())
            .catch(error => console.error(error))
            .then(data => {
                console.log(data);
                this.setState({ loading: false, apiData: data });
            })
  }

  displayName = Home.name

  render() {
    return (
      <div>
        <h1>Recipe Search</h1>


        {/* <Button bsStyle="primary" bsSize="large" onClick={this.tapStart}>Voice Search</Button> */}
        

          <FormGroup controlId="formBasicText">
            <ControlLabel>Write a search query</ControlLabel>
            <FormControl type="input" bsSize="large" value={this.state.text} onChange={this.handleChange} placeholder="recipes with tomato and rice" />
            {/* <Button bsStyle="primary" type="submit" bsSize="large" onClick={this.handleClick}>Search</Button> */}
          </FormGroup>

        
        
        <RecipeList apiData={this.state.apiData} loading={this.state.loading} />

      </div>
    );
  }
}
