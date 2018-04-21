import React, { Component } from 'react';
import { Collapse, Well } from 'react-bootstrap';
import { Recipe } from './Recipe';

export class RecipeList extends Component {
    render() {

        const recipesList = [];

        if (this.props.apiData) {
            this.props.apiData.recipes.forEach(element => {
                recipesList.push(
                    <Recipe key={element.recipeId} recipe={element} />
                );
            });
        }

        return (
            <div>
                <Collapse in={this.props.loading}>
                    <div>
                        <Well> Loading </Well>
                    </div>
                </Collapse>
            
                {recipesList}
            </div>
        );

    }

}