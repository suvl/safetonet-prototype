import React, { Component } from 'react';
import { Button, FormGroup, ControlLabel, FormControl, HelpBlock } from 'react-bootstrap';
import DetectRTC from 'detectrtc';
import RecordRTC from 'recordrtc';
import { RecipeList } from './RecipeList';

export class Popular extends Component {

constructor() {
    super();
    this.state = { 
      loading: true,
      apiData: null
    };

    fetch('http://localhost:5000/api/Search?query=popular')
            .catch(error => console.error(error))
            .then(response => response.json())
            .catch(error => console.error(error))
            .then(data => {
                console.log(data);
                this.setState({ loading: false, apiData: data });
            })
  }

  render() {
    return (
      <div>
        <h1>Popular Recipes</h1>
        
        <RecipeList apiData={this.state.apiData} loading={this.state.loading} />

      </div>
    );
  }
}