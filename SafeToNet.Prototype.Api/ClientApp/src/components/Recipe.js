import React, { Component } from 'react';
import { Media, Grid, Row, Col, Link } from 'react-bootstrap';

export class Recipe extends Component {
    render() {
        const recipe = this.props.recipe;
        return (
            <div>
                <Media>
                    <Media.Left>
                        <img src={recipe.imageUrl} style={{width:'10em',height:'auto'}} />
                    </Media.Left>
                    <Media.Body>
                        <Media.Heading>{recipe.title}</Media.Heading>
                        
                        <Grid>
                            <Row>
                                <Col md={4} xs={4}>Rank: {recipe.socialRank}</Col>
                                <Col md={6} xs={6}>Publisher: {recipe.publisher}</Col>
                            </Row>
                            <Row>
                                <Col md={12}> <a href={recipe.sourceUrl}>Go to recipe</a> </Col>
                            </Row>
                        </Grid>
                        
                    </Media.Body>
                </Media>
            </div>
        )
    }
}